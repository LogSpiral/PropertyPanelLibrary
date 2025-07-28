using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using ReLogic.Content;
using SilkyUIFramework;
using SilkyUIFramework.BasicComponents;
using SilkyUIFramework.BasicElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI;
using SilkyUIFramework.Extensions;
using SilkyUIFramework.Animation;
using PropertyPanelLibrary.BasicElements;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;

public class OptionDefinition : PropertyOption
{
    #region 反射用元数据
    Type EntityDefinitionElementType;//原版DefinitionElement的Type，用来获取它的函数进而套壳
    ConfigElement ElementInstance;
    UIElement OptionChoice;
    SUIProxyElement _proxyOptionChoice;
    MethodInfo createDefinitionMethod;
    MethodInfo tweakDefinitionMethod;
    MethodInfo getDefinitionListMethod;
    MethodInfo getPassedDefinitionListMethod;
    PropertyInfo optionProperty;
    PropertyInfo filterModProperty;
    PropertyInfo filterNameProperty;
    PropertyInfo tooltipProperty;
    MethodInfo setItemMethod;
    #endregion


    bool _switchForward;
    bool _lastCompleteState;
    bool _selectionExpanded;
    bool _pendingUpdateRequired;
    SUIImage _modIcon;
    UIElementGroup _iconContainer;
    SUIEditTextBox _filteringName;
    SUIScrollView _optionView;
    UIElementGroup _mainOptionPanel;
    readonly List<Asset<Texture2D>> modIconTextures = [];
    readonly List<string> modNames = [];
    int currentModIndex = 0;
    IList Options
    {
        get => optionProperty.GetValue(ElementInstance) as IList;
        set => optionProperty.SetValue(ElementInstance, value);
    }
    UIFocusInputTextField FilterMod => filterModProperty.GetValue(ElementInstance) as UIFocusInputTextField;
    UIFocusInputTextField FilterName => filterNameProperty.GetValue(ElementInstance) as UIFocusInputTextField;
    private readonly AnimationTimer _expandTimer = new();
    protected override void CheckAttributes()
    {
        #region 类型检查
        if (!VariableType.IsSubclassOf(typeof(EntityDefinition)))
            throw new Exception($"Type \"{VariableType}\" is not a Definition");
        #endregion

        #region 获取原ConfigElement的Type
        var customItem = GetAttribute<CustomModConfigItemAttribute>();
        if (customItem != null)
            EntityDefinitionElementType = customItem.Type;
        else
        {
            // TML内是这样硬编码的，我也没办法
            Type type = VariableType;
            if (type == typeof(ItemDefinition))
                EntityDefinitionElementType = typeof(ItemDefinitionElement);

            else if (type == typeof(ProjectileDefinition))
                EntityDefinitionElementType = typeof(ProjectileDefinitionElement);

            else if (type == typeof(NPCDefinition))
                EntityDefinitionElementType = typeof(NPCDefinitionElement);

            else if (type == typeof(PrefixDefinition))
                EntityDefinitionElementType = typeof(PrefixDefinitionElement);

            else if (type == typeof(BuffDefinition))
                EntityDefinitionElementType = typeof(BuffDefinitionElement);

            else if (type == typeof(TileDefinition))
                EntityDefinitionElementType = typeof(TileDefinitionElement);

        }
        #endregion

        #region 获取函数
        var methods = EntityDefinitionElementType.GetMethods(BindingFlags.Public |
    BindingFlags.NonPublic |
    BindingFlags.Instance);
        byte bitsByte = 0;
        foreach (var method in methods)
        {
            switch (method.Name)
            {
                case "CreateDefinitionOptionElement":
                    createDefinitionMethod = method;
                    bitsByte += 1;
                    break;
                case "GetPassedOptionElements":
                    getPassedDefinitionListMethod = method;
                    bitsByte += 2;
                    break;
                case "CreateDefinitionOptionElementList":
                    getDefinitionListMethod = method;
                    bitsByte += 4;
                    break;
                case "TweakDefinitionOptionElement":
                    tweakDefinitionMethod = method;
                    bitsByte += 8;
                    break;
            }
            if (bitsByte == 15)
                break;
        }
        #endregion

        #region 获取属性
        optionProperty = EntityDefinitionElementType.GetProperty("Options", BindingFlags.Instance | BindingFlags.NonPublic);
        filterNameProperty = EntityDefinitionElementType.GetProperty("ChooserFilter", BindingFlags.Instance | BindingFlags.NonPublic);
        filterModProperty = EntityDefinitionElementType.GetProperty("ChooserFilterMod", BindingFlags.Instance | BindingFlags.NonPublic);
        #endregion

        base.CheckAttributes();
    }
    protected override void FillOption()
    {
        FitHeight = true;
        LayoutType = LayoutType.Flexbox;
        MainAlignment = MainAlignment.SpaceBetween;
        FitWidth = false;
        FlexDirection = FlexDirection.Row;
        FlexWrap = true;


        ElementInstance = Activator.CreateInstance(EntityDefinitionElementType) as ConfigElement;
        IList list = null;
        int index = -1;
        if (MetaData is ListValueHandler listHandler)
        {
            list = listHandler.List;
            index = listHandler.Index;
        }
        ElementInstance.Bind(MetaData.VariableInfo, MetaData.Item, list, index);
        ElementInstance.OnBind();

        OptionChoice = createDefinitionMethod.Invoke(ElementInstance, []) as UIElement;
        tweakDefinitionMethod.Invoke(ElementInstance, [OptionChoice]);
        var proxyOptionChoice = _proxyOptionChoice = new SUIProxyElement(OptionChoice);
        proxyOptionChoice.LeftMouseClick += delegate
        {
            _selectionExpanded = !_selectionExpanded;
            _pendingUpdateRequired = true;
        };
        proxyOptionChoice.SetTop(2);
        proxyOptionChoice.SetLeft(-4);
        proxyOptionChoice.SetMargin(0);
        proxyOptionChoice.Join(this);


        setItemMethod = OptionChoice.GetType().GetMethod("SetItem", BindingFlags.Instance | BindingFlags.Public);
        tooltipProperty = OptionChoice.GetType().GetProperty("Tooltip", BindingFlags.Instance | BindingFlags.Public);


        _mainOptionPanel = new UIElementGroup();
        _mainOptionPanel.BorderRadius = new(8);
        _mainOptionPanel.SetMargin(4);
        _mainOptionPanel.SetWidth(0, 1);
        _mainOptionPanel.FitHeight = true;
        _mainOptionPanel.FlexWrap = true;
        _mainOptionPanel.MainAlignment = MainAlignment.SpaceBetween;
        _mainOptionPanel.CrossAlignment = CrossAlignment.Center;
        _mainOptionPanel.Join(this);
        modNames.Add("");
        modIconTextures.Add(ModLoader.ManifestAssets.Request<Texture2D>("Terraria.GameContent.UI.States.HubManageMods", AssetRequestMode.ImmediateLoad));
        modNames.Add("Terraria");
        modIconTextures.Add(Main.Assets.Request<Texture2D>("Images/UI/Workshop/HubWorlds", AssetRequestMode.ImmediateLoad));
        foreach (var m in ModLoader.Mods)
        {
            var file = m.File;
            if (file != null && file.HasFile("icon.png"))
            {
                try
                {
                    using (file.Open())
                    using (var s = file.GetStream("icon.png"))
                    {
                        var iconTexture = Main.Assets.CreateUntracked<Texture2D>(s, ".png");

                        if (iconTexture.Width() == 80 && iconTexture.Height() == 80)
                        {
                            modIconTextures.Add(iconTexture);
                            modNames.Add(m.DisplayNameClean);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logging.tML.Error("Unknown error", e);
                }
            }
        }

        var iconContainer = _iconContainer = new UIElementGroup();
        iconContainer.SetWidth(80);
        iconContainer.Join(_mainOptionPanel);
        _modIcon = new SUIImage(modIconTextures[0]);
        _modIcon.ImageScale = new(80f / _modIcon.ImageOriginalSize.X);
        _modIcon.Join(iconContainer);
        iconContainer.LeftMouseClick += (elem, evt) =>
        {
            if (evt.Source != elem) return;
            currentModIndex++;
            currentModIndex %= modIconTextures.Count;
            _modIcon.Texture2D = modIconTextures[currentModIndex];
            FilterMod.SetText(modNames[currentModIndex]);
            _modIcon.ImageScale = new(80f / _modIcon.ImageOriginalSize.X);
            _pendingUpdateRequired = true;
            _switchForward = true;
            _filteringName.InnerText.Text = "";
        };
        iconContainer.RightMouseClick += (elem, evt) =>
        {
            if (evt.Source != elem) return;
            currentModIndex--;
            if (currentModIndex < 0) currentModIndex += modIconTextures.Count;
            currentModIndex %= modIconTextures.Count;
            _modIcon.Texture2D = modIconTextures[currentModIndex];
            FilterMod.SetText(modNames[currentModIndex]);
            _modIcon.ImageScale = new(80f / _modIcon.ImageOriginalSize.X);
            _pendingUpdateRequired = true;
            _switchForward = false;
            _filteringName.InnerText.Text = "";
        };

        _filteringName = new SUIEditTextBox()
        {
            BackgroundColor = Color.Black * .3f,
            BorderRadius = new(8)
        };
        _filteringName.FitWidth = false;
        _filteringName.SetWidth(-90, 1);
        _filteringName.SetMargin(4);
        _filteringName.InnerText.ContentChanged += (sender, arg) =>
        {
            FilterName.SetText(arg.Text);
            _pendingUpdateRequired = true;
        };
        _filteringName.RightMouseClick += delegate
        {
            FilterName.SetText("");
            _pendingUpdateRequired = true;
        };
        _filteringName.Join(_mainOptionPanel);
        _filteringName.BackgroundColor = default;

        _optionView = new SUIScrollView();
        _optionView.SetPadding(0f, 0f);
        _optionView.SetWidth(0, 1);
        _optionView.Join(_mainOptionPanel);
        _optionView.SetMargin(8);
        UpdateTimerVisuals();
    }
    void UpdateTimerVisuals()
    {
        float factor = _expandTimer.Schedule;
        _optionView.SetHeight(200 * factor);
        _iconContainer.SetHeight(80 * factor);
        _modIcon.ImageColor = _expandTimer.Lerp(default, Color.White);
        _filteringName.SetHeight(40 * factor);
        _filteringName.BackgroundColor = _expandTimer.Lerp(default, Color.Black * .4f);
        _optionView.ScrollBar.BackgroundColor = _expandTimer.Lerp(default, Color.Black * .25f);
        _optionView.ScrollBar.BarColor.Default = _expandTimer.Lerp(default, Color.Black * .2f);
        _optionView.SetMargin(8 * factor);
        _filteringName.SetMargin(4 * factor);
        _mainOptionPanel.SetMargin(4 * factor);
    }
    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);
        _lastCompleteState = _expandTimer.IsCompleted;
        _expandTimer.Update(gameTime);

        if (!_lastCompleteState)
            UpdateTimerVisuals();


        if (!_pendingUpdateRequired) return;
        _pendingUpdateRequired = false;
        //_mainOptionPanel.Remove();
        if (_selectionExpanded)
        {
            SetUpList();
        }

        if (_selectionExpanded)
            _expandTimer.StartUpdate();
        else
            _expandTimer.StartReverseUpdate();
    }
    void SetUpList()
    {
        _optionView.Container.RemoveAllChildren();
        //Options ??= getDefinitionListMethod.Invoke(ElementInstance, []) as IList;
        Options ??= getDefinitionListMethod.Invoke(ElementInstance, []) as IList;

        IList passed = getPassedDefinitionListMethod.Invoke(ElementInstance, []) as IList;
        List<UIElement> passedElem = [];
        int c = 0;
        foreach (var p in passed)
        {
            if (p is not UIElement element) return;
            SUIProxyElement proxy = new(element);
            proxy.LeftMouseClick += delegate
            {
                var prop = p.GetType().GetProperty("Definition");
                SetValue(prop.GetValue(p));
                _pendingUpdateRequired = true;
                _selectionExpanded = false;
                setItemMethod?.Invoke(OptionChoice, [GetValue()]);
                _filteringName.InnerText.Text = "";
            };
            proxy.Join(_optionView.Container);
            c++;
        }

        if (c == 0 && _filteringName.InnerText.Text == "")
        {
            modIconTextures.RemoveAt(currentModIndex);
            modNames.RemoveAt(currentModIndex);
            if (_switchForward)
            {
                if (currentModIndex >= modIconTextures.Count)
                    currentModIndex = 0;
            }
            else
            {
                currentModIndex--;
                if (currentModIndex < 0)
                    currentModIndex += modIconTextures.Count;
            }

            _modIcon.Texture2D = modIconTextures[currentModIndex];
            FilterMod.SetText(modNames[currentModIndex]);
            _modIcon.ImageScale = new(80f / _modIcon.ImageOriginalSize.X);
            SetUpList();
        }
    }
    protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (_proxyOptionChoice.IsMouseHovering)
            UICommon.TooltipMouseText(tooltipProperty.GetValue(OptionChoice).ToString());
        else if (_iconContainer.IsMouseHovering)
            UICommon.TooltipMouseText(currentModIndex == 0 ? "All" : modNames[currentModIndex]);
        else
            foreach (var elem in _optionView.Container.Children)
                if (elem.IsMouseHovering)
                {
                    UICommon.TooltipMouseText(tooltipProperty.GetValue((elem as SUIProxyElement).InnerElement).ToString());
                    break;
                }

        base.Draw(gameTime, spriteBatch);
    }

    // TODO 对于外部写入值时的处理？
    //protected override void OnSetValueExternal(object value)
    //{
    //    UpdateNeeded = true;
    //    SelectionExpanded = false;
    //    setItemMethod.Invoke(OptionChoice, [GetValue()]);
    //}

    public override string Label => base.Label + ":" + tooltipProperty?.GetValue(OptionChoice).ToString() ?? "";

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegistreOptionToTypeComplex(this, type => type.IsSubclassOf(typeof(EntityDefinition)));
    }
}