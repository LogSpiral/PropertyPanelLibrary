using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropertyPanelLibrary.BasicElements;
using PropertyPanelLibrary.Graphics2D;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System;
using System.Linq;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI.Chat;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;

public class OptionDropdownList : PropertyOption
{
    private bool DropdownListPersists => DropdownListUI.Container.DropdownCaller == this;
    private UIElementGroup _textBox;
    private UITextView _textElement;
    private string[] _valueStrings;
    private string[] _valueTooltips;
    private bool IsStringOption;
    private float _maxTextWidth;
    private static SUIDropdownListContainer DropdownList => DropdownListUI.Container;

    protected override void FillOption()
    {
        OverflowHidden = true;

        CheckValid();

        var box = new UIElementGroup
        {
            FitWidth = true
        };
        box.SetHeight(0, 1);
        box.Join(this);

        GetOptions();

        _textBox = new UIElementGroup
        {
            BackgroundColor = Color.Black * 0.3f,
            BorderRadius = new Vector4(6f)
        };
        _textBox.SetTop(0, 0, 0.5f);
        _textBox.LeftMouseDown += delegate
        {
            if (!Interactable) return;

            SoundEngine.PlaySound(SoundID.MenuTick);
            var dimensions = _textBox.Bounds;
            float width = _textBox.Bounds.Width;
            var dropdownList = DropdownList;
            dropdownList.BuildDropdownList(0, 29, true, width, _valueStrings, GetString(), _textBox);
            dropdownList.OptionSelectedCallback = s =>
            {
                if (IsStringOption)
                {
                    SetValue(s);
                    return;
                }

                int index = Array.IndexOf(_valueStrings, s);
                SetConfigValue(index);
            };
            //dropdownList.DrawCallback = () =>
            //{
            //    // TooltipPanel.SetText(Tooltip);
            //    // TODO MouseHandler加入MouseHover
            //};
            //dropdownList.HoverOnOptionCallback = s =>
            //{
            //    int index = Array.IndexOf(_valueStrings, s);
            //    string tooltip = _valueTooltips[index];
            //    if (!string.IsNullOrWhiteSpace(tooltip))
            //        TooltipPanel.SetText($"{Tooltip}\n[c/0099ff:{s}]: {tooltip}");
            //    // 这里不需要else了，因为DrawCallback会先执行
            //};
        };
        float width = Math.Min(320, _maxTextWidth + 48);
        _textBox.SetPadding(0); // Padding影响里面的文字绘制
        _textBox.SetSize(width, 28);
        _textBox.Join(box);

        _textElement = new UITextView()
        {
            Text = GetString()
        };
        _textElement.SetTop(0, 0, 0.5f);
        _textElement.SetLeft(8);
        _textElement.Join(_textBox);
    }

    private void GetOptions()
    {
        if (!IsStringOption)
            _valueStrings = Enum.GetNames(VariableType);
        _valueTooltips = new string[_valueStrings.Length];

        if (!IsStringOption)
            for (int i = 0; i < _valueStrings.Length; i++)
            {
                var enumFieldFieldInfo = VariableType.GetField(_valueStrings[i]);
                if (enumFieldFieldInfo is null)
                    continue;

                string name = ConfigManager.GetLocalizedLabel(new PropertyFieldWrapper(enumFieldFieldInfo));
                _valueStrings[i] = name;
                string tooltip = ConfigManager.GetLocalizedTooltip(new PropertyFieldWrapper(enumFieldFieldInfo));
                _valueTooltips[i] = tooltip;
            }

        _maxTextWidth = _valueStrings.Max(i => ChatManager.GetStringSize(FontAssets.MouseText.Value, i, Vector2.One).X);
    }
    protected override void CheckAttributes()
    {
        OptionLabelsAttribute = GetAttribute<OptionStringsAttribute>();
        base.CheckAttributes();
    }
    protected OptionStringsAttribute OptionLabelsAttribute { get; set; }
    private void CheckValid()
    {
        if (OptionLabelsAttribute is { } strOptionAttribute)
        {
            _valueStrings = strOptionAttribute.OptionLabels;
            IsStringOption = true;
            return;
        }
        if (!VariableType.IsEnum && !IsStringOption)
            throw new Exception($"Type \"{VariableType}\" is not a enum type");
    }

    private void SetConfigValue(int index)
    {
        //if (!Interactable) return;
        var value = Enum.GetValues(VariableType).GetValue(index);
        SetValue(value);
        //ConfigHelper.SetConfigValue(Config, VariableInfo, value, Item, broadcast, path: path);
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);

        // 就算没有Hover，要是下拉框打开了也要高光（调整AnimationTimer）
        //if (!IsMouseHovering)
        //{
        //    if (DropdownListPersists)
        //    {
        //        HoverTimer.ImmediateCompleted();
        //    }
        //}

        _textBox.IgnoreMouseInteraction = !Interactable;
        _textBox.BackgroundColor = _textBox.HoverTimer.Lerp(Color.Black * 0.4f, Color.Black * 0.2f);
        _textElement.Text = GetString();
    }

    protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(gameTime, spriteBatch);
        if (Height.Pixels is 0) return;

        var bounds = _textBox.Bounds;
        SDFGraphics.NoBorderTriangleIsosceles(
            bounds.Position + new Vector2(bounds.Width - 20, bounds.Height * .5f),
            new Vector2(.5f),
            new Vector2(24, DropdownListPersists ? -12 : 12),
            Color.White,
            SDFGraphics.GetMatrix(true));
    }

    private int GetIndex() => IsStringOption ? Array.IndexOf(_valueStrings, GetValue()) : Array.IndexOf(Enum.GetValues(VariableType), GetValue());

    private string GetString() => IsStringOption ? GetValue()?.ToString() ?? "" : _valueStrings[GetIndex()];

    protected override void Register(Mod mod)
    {
        base.Register(mod);

        PropertyOptionSystem.RegistreOptionToTypeComplex(this, type => type.IsEnum);
    }
}