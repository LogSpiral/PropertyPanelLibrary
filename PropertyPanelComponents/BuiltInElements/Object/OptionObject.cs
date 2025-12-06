using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using PropertyPanelLibrary.BasicElements;
using PropertyPanelLibrary.PropertyPanelComponents.Attributes;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.Writers;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using SilkyUIFramework;
using SilkyUIFramework.Layout;
using SilkyUIFramework.Animation;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Object;

public class OptionObject : PropertyOption
{
    protected UIElementGroup ButtonContainer { get; set; }
    protected PropertyPanel PropertyPanel { get; private set; }
    private SUITriangleIcon InitializeButton { get; set; }
    private SUITriangleToggle ExpandButton { get; set; }
    private SUICross DeleteButton { get; set; }
    protected bool pendingChanges;

    protected override void FillOption()
    {
        FitHeight = true;
        LayoutType = LayoutType.Flexbox;
        MainAlignment = MainAlignment.SpaceBetween;
        FlexDirection = FlexDirection.Row;
        FlexWrap = true;
        BuildButtonContainer();
        BuildPropertyPanel();
        BuildInitiateButton();
        BuildExpandButton();
        BuildDeleteButton();
        InitializePanel();
    }

    private void BuildButtonContainer()
    {
        ButtonContainer = new UIElementGroup
        {
            FitWidth = true,
            FitHeight = true,
            Margin = new Margin(4f),
        };
        ButtonContainer.Join(this);
    }

    private void BuildPropertyPanel()
    {
        PropertyPanel = new PropertyPanel();
        PropertyPanel.SetWidth(0, 1);
        PropertyPanel.Join(this);

        DelegateWriter writeObserver = new DelegateWriter();
        writeObserver.OnWriteValue += delegate
        {
            SetValue(GetValue());
        };
        PropertyPanel.Writer = new CombinedWriter(DefaultWriter.Instance, writeObserver);

        // PropertyPanel.Decorator = FitHeightDecorator.Instance;
    }

    private void BuildInitiateButton()
    {
        InitializeButton = new SUITriangleIcon();
        InitializeButton.LeftMouseClick += delegate
        {
            SoundEngine.PlaySound(SoundID.Tink);
            object data = Activator.CreateInstance(VariableType, true);
            string json = JsonDefaultValueAttribute?.Json ?? "{}";
            JsonConvert.PopulateObject(json, data, ConfigManager.serializerSettings);
            SetValue(data);
            pendingChanges = true;
        };
    }

    private void BuildExpandButton()
    {
        ExpandButton = new SUITriangleToggle(expanded);
        ExpandButton.LeftMouseClick += delegate
        {
            expanded = ExpandButton.State;
            if (expanded) _expandTimer.StartUpdate();
            else _expandTimer.StartReverseUpdate();
        };
    }

    private void BuildDeleteButton()
    {
        DeleteButton = new SUICross(SUIColor.Warn * .5f, SUIColor.Warn);
        DeleteButton.SetSize(25, 25);
        DeleteButton.Margin = new Margin(4f, 0, 4, 0);
        DeleteButton.BackgroundColor = Color.Black * .4f;
        DeleteButton.BorderRadius = new Vector4(4f);
        DeleteButton.LeftMouseClick += delegate
        {
            DeleteProcess();
        };
    }

    protected virtual void DeleteProcess()
    {
        SetValue(null);
        pendingChanges = true;
    }

    protected virtual bool ShouldAppendDeleteButton() => NullAllowedAttribute != null;

    private void InitializePanel()
    {
        if (expanded)
            _expandTimer.ImmediateCompleted();
        //var target = GetValue();
        //if (target == null)
        //    InitializeButton.Join(ButtonContainer);
        //else
        //    PropertyPanel.Filler =
        //        new ObjectMetaDataFiller(target)
        //        .SetAsSubOption(this, MetaData.Item, MetaData.VariableInfo);
        pendingChanges = true;
        UpdateTimerVisuals();
    }

    protected override void CheckAttributes()
    {
        if (GetAttribute<ExpandAttribute>() is { } expandedAttribute)
            expanded = expandedAttribute.Expand;

        if (GetAttribute<JsonDefaultValueAttribute>() is { } jsonDefaultValue)
            JsonDefaultValueAttribute = jsonDefaultValue;

        if (GetAttribute<NullAllowedAttribute>() is { } nullAllowed)
            NullAllowedAttribute = nullAllowed;

        if (GetAttribute<SeparatePageAttribute>() is { } separatePage)
            SeparatePageAttribute = separatePage;

        if (GetAttribute<RangeAttribute>() is { } range)
            RangeAttribute = range;

        if (GetAttribute<IncrementAttribute>() is { } increment)
            IncrementAttribute = increment;

        if (GetAttribute<LabelPresentValueAttribute>() is { } present)
            ShowStringValueInLabel = present.IsPresent;
        else
            ShowStringValueInLabel = VariableType.GetMethod("ToString", []).DeclaringType != typeof(object);
        base.CheckAttributes();
    }

    public override void CheckDesignagedAttributes(HashSet<Attribute> attributes)
    {
        foreach (var attribute in attributes)
        {
            switch (attribute)
            {
                case ExpandAttribute expandAttribute:
                    expanded = expandAttribute.Expand;
                    break;

                case JsonDefaultValueAttribute jsonDefaultValue:
                    JsonDefaultValueAttribute ??= jsonDefaultValue;
                    break;

                case NullAllowedAttribute nullAllowed:
                    NullAllowedAttribute ??= nullAllowed;
                    break;

                case SeparatePageAttribute separatePage:
                    SeparatePageAttribute ??= separatePage;
                    break;

                case RangeAttribute range:
                    RangeAttribute ??= range;
                    break;

                case IncrementAttribute increment:
                    IncrementAttribute = increment;
                    break;
            }
        }
        base.CheckDesignagedAttributes(attributes);
    }

    public override string Label => base.Label + (ShowStringValueInLabel ? $":{GetValue()?.ToString() ?? "null"}" : "");
    protected bool expanded;
    protected bool ShowStringValueInLabel;

    #region Attributes

    protected JsonDefaultValueAttribute JsonDefaultValueAttribute;
    protected NullAllowedAttribute NullAllowedAttribute;
    protected SeparatePageAttribute SeparatePageAttribute;
    protected RangeAttribute RangeAttribute;
    protected IncrementAttribute IncrementAttribute;

    #endregion Attributes

    protected AnimationTimer _expandTimer = new();

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegistreOptionToTypeComplex(this, type =>
        {
            if (type.IsArray) return false;
            if (type.IsGenericType)
            {
                var definition = type.GetGenericTypeDefinition();
                if (definition == typeof(List<>)) return false;
                if (definition == typeof(Dictionary<,>)) return false;
                if (definition == typeof(HashSet<>)) return false;
            }
            return true;
        });
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        _expandTimer.Update(gameTime);
        _heightTimer.Update(gameTime);
        UpdateTimerVisuals();
        HandlePendingChanges();
        base.UpdateStatus(gameTime);
    }

    public override void HandleUpdateStatus(GameTime gameTime)
    {
        bool updateHeight = pendingChanges;
        base.HandleUpdateStatus(gameTime);
        if (_pendingUpdateHeight)
        {
            _pendingUpdateHeight = false;
            SetTargetHeight(
                MathF.Min(
                    InnerPanelMaxHeight,
                    PropertyPanel.OptionList.Container.OuterBounds.Height
                    ));
        }
        _pendingUpdateHeight = updateHeight;
    }

    private void UpdateTimerVisuals()
    {
        float factor = _expandTimer.Schedule;
        _targetHeight = _heightTimer.Lerp(_lastTargetHeight, _currentTargetHeight);
        PropertyPanel.SetHeight((_targetHeight + 16) * factor, 0);
        PropertyPanel.SetPadding(8f * factor);
        // Main.NewText((_lastTargetHeight, _currentTargetHeight,_targetHeight));
    }

    protected float InnerPanelMaxHeight { get; set; } = 200;
    private AnimationTimer _heightTimer = new();
    private float _targetHeight;
    private float _currentTargetHeight;
    private float _lastTargetHeight;
    private bool _pendingUpdateHeight;

    protected void SetTargetHeight(float currentTargetHeight)
    {
        _heightTimer.StartUpdate(true);
        _lastTargetHeight = _currentTargetHeight;
        _currentTargetHeight = currentTargetHeight;
    }

    private void HandlePendingChanges()
    {
        if (pendingChanges)
        {
            pendingChanges = false;
            var data = GetValue();

            InitializeButton.RemoveFromParent();
            ExpandButton.RemoveFromParent();
            DeleteButton.RemoveFromParent();

            if (data != null)
            {
                ExpandButton.Join(ButtonContainer);

                if (ShouldAppendDeleteButton())
                    DeleteButton.Join(ButtonContainer);
                PropertyPanel.Filler = GetInternalPanelFiller(data);
            }
            else
            {
                InitializeButton.Join(ButtonContainer);
                PropertyPanel.Filler = NoneFiller.Instance;
            }
        }
    }

    protected virtual IPropertyOptionFiller GetInternalPanelFiller(object data)
    {
        return new ObjectMetaDataFiller(data)
                    .SetAsSubOption(
                        this,
                        MetaData.Item,
                        MetaData.VariableInfo);
    }
}