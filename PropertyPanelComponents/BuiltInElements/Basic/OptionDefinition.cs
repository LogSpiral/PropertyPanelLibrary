using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropertyPanelLibrary.BasicElements;
using PropertyPanelLibrary.EntityDefinition;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using ReLogic.Content;
using SilkyUIFramework;
using SilkyUIFramework.Layout;
using SilkyUIFramework.Animation;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using EDefinition = Terraria.ModLoader.Config.EntityDefinition;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;

public partial class OptionDefinition : PropertyOption
{
    // 是否处于反射代理模式
    public bool IsReflectProxyMode { get; set; }

    #region 自定义处理器
    private IEntityDefinitionHandler Handler { get; set; }
    private ReflectProxyModeManager ProxyManager { get; set; }
    #endregion

    #region 自身逻辑控制
    private bool _switchForward;
    private bool _lastCompleteState;
    private bool _selectionExpanded;
    private bool _pendingUpdateRequired;
    private SUIImage _modIcon;
    private UIElementGroup _iconContainer;
    private SUIEditTextBox _filteringName;
    private SUIScrollView _optionView;
    private UIElementGroup _mainOptionPanel;
    private readonly List<Asset<Texture2D>> modIconTextures = [];
    private readonly List<string> modNames = [];
    private int currentModIndex = 0;
    private readonly AnimationTimer _expandTimer = new();
    #endregion




    protected override void CheckAttributes()
    {
        #region 类型检查

        if (!VariableType.IsSubclassOf(typeof(Terraria.ModLoader.Config.EntityDefinition)))
            throw new Exception($"Type \"{VariableType}\" is not a Definition");

        #endregion 类型检查

        #region 获取原ConfigElement的Type

        if (GetAttribute<CustomEntityDefinitionHandlerAttribute>() is { } handlerAttribute && handlerAttribute.Handler is { } handler)
        {
            Handler = handler;
            IsReflectProxyMode = false;
        }
        else
        {
            Type entityDefinitionElementType = null;
            if (GetAttribute<CustomModConfigItemAttribute>() is { } customItem)
            {
                entityDefinitionElementType = customItem.Type;
                IsReflectProxyMode = true;
            }
            else
            {
                // TML内是这样硬编码的，我也没办法
                var type = VariableType;
                if (type == typeof(ItemDefinition))
                    entityDefinitionElementType = typeof(ItemDefinitionElement);
                else if (type == typeof(ProjectileDefinition))
                    entityDefinitionElementType = typeof(ProjectileDefinitionElement);
                else if (type == typeof(NPCDefinition))
                    entityDefinitionElementType = typeof(NPCDefinitionElement);
                else if (type == typeof(PrefixDefinition))
                    entityDefinitionElementType = typeof(PrefixDefinitionElement);
                else if (type == typeof(BuffDefinition))
                    entityDefinitionElementType = typeof(BuffDefinitionElement);
                else if (type == typeof(TileDefinition))
                    entityDefinitionElementType = typeof(TileDefinitionElement);
                IsReflectProxyMode = true;
            }
            ProxyManager = new ReflectProxyModeManager(entityDefinitionElementType);
        }


        #endregion 获取原ConfigElement的Type



        base.CheckAttributes();
    }

    protected override void FillOption()
    {
        #region 自身初始化
        FitHeight = true;
        LayoutType = LayoutType.Flexbox;
        MainAlignment = MainAlignment.SpaceBetween;
        FitWidth = false;
        FlexDirection = FlexDirection.Row;
        FlexWrap = true;
        #endregion



        ProxyManager?.AppendOptionChoice(this);
        if (Handler?.CreateChoiceView(MetaData) is { } view)
        {
            view.LeftMouseClick += delegate
            {
                _selectionExpanded = !_selectionExpanded;
                _pendingUpdateRequired = true;
            };
            view.Join(this);
        }



        #region 选项面板
        _mainOptionPanel = new UIElementGroup
        {
            BorderRadius = new Vector4(8)
        };
        _mainOptionPanel.SetPadding(4);
        _mainOptionPanel.SetWidth(0, 1);
        _mainOptionPanel.FitHeight = true;
        _mainOptionPanel.FlexWrap = true;
        _mainOptionPanel.MainAlignment = MainAlignment.SpaceBetween;
        _mainOptionPanel.CrossAlignment = CrossAlignment.Center;
        _mainOptionPanel.Join(this);
        #endregion

        #region 加载模组名和模组图标
        modNames.Add("");
        modIconTextures.Add(ModLoader.ManifestAssets.Request<Texture2D>("Terraria.GameContent.UI.States.HubManageMods", AssetRequestMode.ImmediateLoad));
        modNames.Add("Terraria");
        modIconTextures.Add(Main.Assets.Request<Texture2D>("Images/UI/Workshop/HubWorlds", AssetRequestMode.ImmediateLoad));
        foreach (var m in ModLoader.Mods)
        {
            var file = m.File;
            if (file == null || !file.HasFile("icon.png")) continue;
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
        #endregion

        #region 图标容器
        var iconContainer = _iconContainer = new UIElementGroup();
        iconContainer.SetWidth(80);
        iconContainer.Join(_mainOptionPanel);
        _modIcon = new SUIImage(modIconTextures[0])
        {
            IgnoreMouseInteraction = true
        };
        _modIcon.ImageScale = new Vector2(80f / _modIcon.ImageOriginalSize.X);
        _modIcon.Join(iconContainer);
        iconContainer.LeftMouseClick += (elem, evt) =>
        {
            if (evt.Source != elem) return;
            currentModIndex++;
            currentModIndex %= modIconTextures.Count;
            _modIcon.Texture2D = modIconTextures[currentModIndex];

            ProxyManager?.SetModFilter(modNames[currentModIndex]);
            Handler?.SetModFilter(modNames[currentModIndex]);

            _modIcon.ImageScale = new Vector2(80f / _modIcon.ImageOriginalSize.X);
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

            ProxyManager?.SetModFilter(modNames[currentModIndex]);
            Handler?.SetModFilter(modNames[currentModIndex]);

            _modIcon.ImageScale = new Vector2(80f / _modIcon.ImageOriginalSize.X);
            _pendingUpdateRequired = true;
            _switchForward = false;
            _filteringName.InnerText.Text = "";
        };
        #endregion

        #region 筛选栏
        _filteringName = new SUIEditTextBox
        {
            BackgroundColor = Color.Black * .3f,
            BorderRadius = new Vector4(8),
            FitWidth = false
        };
        _filteringName.SetWidth(-90, 1);
        _filteringName.SetPadding(4);
        _filteringName.InnerText.ContentChanged += (sender, arg) =>
        {
            ProxyManager?.SetNameFilter(arg.Text);
            Handler?.SetNameFilter(arg.Text);

            _pendingUpdateRequired = true;
        };
        _filteringName.RightMouseClick += delegate
        {
            ProxyManager?.SetNameFilter("");
            Handler?.SetNameFilter("");

            _pendingUpdateRequired = true;
        };
        _filteringName.Join(_mainOptionPanel);
        _filteringName.BackgroundColor = default;
        #endregion

        #region 选项列表
        _optionView = new SUIScrollView();
        _optionView.SetPadding(0f, 0f);
        _optionView.SetWidth(0, 1);
        _optionView.Join(_mainOptionPanel);
        _optionView.SetPadding(8);
        _optionView.Container.CrossAlignment = CrossAlignment.Center;
        _optionView.Container.MainAlignment = MainAlignment.Start;
        #endregion

        UpdateTimerVisuals();
    }

    private void UpdateTimerVisuals()
    {
        float factor = _expandTimer.Schedule;
        _optionView.SetHeight(200 * factor);
        _iconContainer.SetHeight(80 * factor);
        _modIcon.ImageColor = _expandTimer.Lerp(default, Color.White);
        _filteringName.SetHeight(40 * factor);
        _filteringName.BackgroundColor = _expandTimer.Lerp(default, Color.Black * .4f);
        _optionView.ScrollBar.BackgroundColor = _expandTimer.Lerp(default, Color.Black * .25f);
        _optionView.ScrollBar.BarColor.Default = _expandTimer.Lerp(default, Color.Black * .2f);
        _optionView.SetPadding(8 * factor);
        _filteringName.SetPadding(4 * factor);
        _mainOptionPanel.SetPadding(4 * factor);
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
            SetUpList();


        if (_selectionExpanded)
            _expandTimer.StartUpdate();
        else
            _expandTimer.StartReverseUpdate();
    }

    private void SetUpList()
    {
        _optionView.Container.RemoveAllChildren();

        bool isEmpty = true;
        ProxyManager?.SetupList(this, out isEmpty);
        if (Handler != null)
        {
            void LeftChickOption(UIView elem, SilkyUIFramework.UIMouseEvent evt)
            {
                SetValue(Handler.GetDefinitionFromOption(elem));
                _pendingUpdateRequired = true;
                _selectionExpanded = false;
                Handler.SetDefinitionToOption(GetValue() as EDefinition);
                _filteringName.InnerText.Text = "";
            }
            Handler.InitializeOptionList(LeftChickOption);
            var passed = Handler.GetPassedOptionElements();
            foreach (var p in passed)
            {
                p.Join(_optionView.Container);
                isEmpty = false;
            }
        }


        #region 跳过空的模组选项
        if (isEmpty && _filteringName.InnerText.Text == "")
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

            ProxyManager?.SetModFilter(modNames[currentModIndex]);
            Handler?.SetModFilter(modNames[currentModIndex]);

            _modIcon.Texture2D = modIconTextures[currentModIndex];
            _modIcon.ImageScale = new Vector2(80f / _modIcon.ImageOriginalSize.X);
            SetUpList();
        }
        #endregion
    }

    protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (_iconContainer.IsMouseHovering)
            UICommon.TooltipMouseText(string.IsNullOrEmpty(modNames[currentModIndex]) ? "All" : modNames[currentModIndex]);
        else
        {
            ProxyManager?.HoverText(this);
            Handler?.HandleHoverText();
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

    public override string Label => base.Label;//+ ":" + tooltipProperty?.GetValue(OptionChoice).ToString() ?? "";

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegistreOptionToTypeComplex(this, type => type.IsSubclassOf(typeof(Terraria.ModLoader.Config.EntityDefinition)));
    }
}