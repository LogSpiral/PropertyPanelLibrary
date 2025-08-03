using Microsoft.Xna.Framework;
using PropertyPanelLibrary.BasicElements;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework;
using SilkyUIFramework.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;

public class OptionSlider : PropertyOption
{
    public static Type[] SupportedTypes { get; } =
        [
        typeof(sbyte),
        typeof(byte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(decimal)
        ];

    public static Type[] FractionTypes { get; } =
        [
        typeof(float),
        typeof(double),
        typeof(decimal)
        ];

    protected override void Register(Mod mod)
    {
        foreach (var type in SupportedTypes)
            PropertyOptionSystem.RegisterOptionToType(this, type);
        base.Register(mod);
    }

    private bool IsFractional => FractionTypes.Contains(VariableType);
    private bool IsInt => !IsFractional;

    private void SetConfigValue(double value, bool broadcast)
    {
        object realValue = Convert.ChangeType(IsFractional ? value : Math.Round(value), VariableType);
        SetValue(realValue, broadcast);
    }

    private Utils.ColorLerpMethod _colorLerpMethod;

    public void SetColorMethod(Utils.ColorLerpMethod colorMethod)
    {
        _colorLerpMethod = colorMethod;
        _slideBox.ColorMethod = colorMethod;
    }

    public void SetColorPendingModified() => _slideBox.colorMethodPendingModified = true;

    private SUISplitButton _splitButton;
    private SUISlideBox _slideBox;
    private SUINumericTextBox _numericTextBox;

    protected override void FillOption()
    {
        var box = new UIElementGroup();
        box.SetLeft(0, 0, 1);
        box.SetTop(0, 0, 0.5f);
        box.SetHeight(0, 1);
        box.FitWidth = true;
        box.Join(this);
        box.SetMargin(4);
        box.CrossAlignment = CrossAlignment.Start;
        AddUpDown(box);
        AddTextBox(box);
        AddSlideBox(box);
    }

    internal double Min = 0;
    internal double Max = 1;
    internal double Default = 1;
    internal double? Increment = null;
    private RangeAttribute _rangeAttribute;
    private DefaultValueAttribute _defaultValueAttribute;
    private IncrementAttribute _incrementAttribute;
    private CustomModConfigItemAttribute _customConfigAttribute;
    private SliderColorAttribute _sliderColorAttribute;
    protected override void CheckAttributes()
    {
        base.CheckAttributes();
        var type = VariableType;
        var pair = (Min, Max);
        if (IsInt)
            pair = (0.0, 100.0);

        if (type == typeof(byte))
            pair = (0.0, 255.0);
        else if (type == typeof(sbyte))
            pair = (-128.0, 127.0);
        /*if (MetaData.VariableInfo?.IsProperty ?? false) //如果是属性就可以玩得花一点(x
        {
            if (type == typeof(short))
                pair = (-32768.0, 32767.0);
            else if (type == typeof(ushort))
                pair = (0.0, 65536.0);
            else if (type == typeof(int))
                pair = (-2147483648.0, 2147483647.0);
            else if (type == typeof(uint))
                pair = (0.0, 4294967295);
        }*/
        (Min, Max) = pair;
        var rangeAttribute = _rangeAttribute;
        if (GetAttribute<RangeAttribute>() is { } rangeAtt)
            rangeAttribute = _rangeAttribute = rangeAtt;
        if (rangeAttribute != null)
        {
            Max = Convert.ToDouble(rangeAttribute.Max);
            Min = Convert.ToDouble(rangeAttribute.Min);
        }

        var defaultValueAttribute = _defaultValueAttribute;
        if (GetAttribute<DefaultValueAttribute>() is { } defaultValue)
            defaultValueAttribute = _defaultValueAttribute = defaultValue;
        if (defaultValueAttribute != null)
            Default = Convert.ToDouble(defaultValueAttribute.Value);

        var incrementAttribute = _incrementAttribute;
        if (GetAttribute<IncrementAttribute>() is { } increment)
            incrementAttribute = _incrementAttribute = increment;
        if (incrementAttribute != null)
            Increment = Convert.ToDouble(incrementAttribute.Increment);
        if (Increment == null && IsInt)
            Increment = 1.0;

        var customConfigAttribute = _customConfigAttribute;
        if (GetAttribute<CustomModConfigItemAttribute>() is { } customConfig)
            customConfigAttribute = _customConfigAttribute = customConfig;
        if (customConfigAttribute != null)
        {
            var elem = Activator.CreateInstance(customConfigAttribute.Type);
            if (elem is RangeElement range)
            {
                if (range.ColorMethod.Invoke(0.0f) != Color.Black && range.ColorMethod.Invoke(1.0f) != Color.White)
                    _colorLerpMethod = range.ColorMethod;
            }
        }
        var sliderColorAttribute = _sliderColorAttribute;
        if (GetAttribute<SliderColorAttribute>() is { } slideColor)
            sliderColorAttribute = _sliderColorAttribute = slideColor;
        if (_colorLerpMethod == null && sliderColorAttribute != null)
            _colorLerpMethod = t => Color.Lerp(Color.Black * 0.3f, sliderColorAttribute.Color, t * t);
    }
    public override void CheckDesignagedAttributes(HashSet<Attribute> attributes)
    {
        foreach (var attribute in attributes)
        {
            switch (attribute)
            {
                case RangeAttribute range:
                    _rangeAttribute ??= range;
                    break;
                case DefaultValueAttribute defaultValue:
                    _defaultValueAttribute ??= defaultValue;
                    break;
                case IncrementAttribute increment:
                    _incrementAttribute ??= increment;
                    break;
                case CustomModConfigItemAttribute customConfig:
                    _customConfigAttribute ??= customConfig;
                    break;
                case SliderColorAttribute sliderColor:
                    _sliderColorAttribute ??= sliderColor;
                    break;
            }
        }
    }
    private void AddUpDown(UIElementGroup box)
    {
        _splitButton = new SUISplitButton()
        {
            buttonBorderColor = Color.Black,
            buttonColor = Color.Black * 0.4f,
        };
        _splitButton.SetSize(25, 25);
        _splitButton.SetTop(0, 0, 0.5f);
        _splitButton.SetMargin(4f);
        _splitButton.Margin = new(4f);
        _splitButton.LeftMouseDown += (sender, evt) =>
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            var btn = sender as SUISplitButton;
            double value = Min + (Max - Min) * _slideBox.Value + (btn.IsUP ? 1 : -1) * Increment.Value;
            value = Math.Clamp(value, Min, Max);
            SetConfigValue(value, broadcast: true);
        };
        _splitButton.Join(box);
    }

    private void AddTextBox(UIElementGroup box)
    {
        bool isInt = IsInt;
        _numericTextBox = new SUINumericTextBox
        {
            MinValue = Min,
            MaxValue = Max,
            InnerText =
            {
                TextAlign = new Vector2(0.5f, 0.5f),
                TextOffset = new Vector2(0f, -2f),

                MaxWordLength = isInt ? 12 : 4,
                MaxLines = 1,
                WordWrap  = false
            },
            MaxLength = isInt ? 12 : 4,
            DefaultValue = Default,
            Format = isInt ? "0" : "0.00"
        };
        _numericTextBox.SetTop(0, 0, 0.5f);
        _numericTextBox.BorderRadius = new(8f);
        _numericTextBox.InnerText.ContentChanged += (sender, arg) =>
        {
            if (!double.TryParse(arg.Text, out var value))
                return;
            if (!sender.IsFocus) return;
            SetConfigValue(value, broadcast: false);
        };
        _numericTextBox.InnerText.EndTakingInput += (sender, arg) =>
        {
            if (!_numericTextBox.IsValueSafe)
                return;
            var value = _numericTextBox.Value;
            value = Math.Clamp(value, Min, Max);
            SetConfigValue(value, broadcast: true);
        };
        _numericTextBox.SetPadding(2);
        _numericTextBox.Margin = new(4f);
        _numericTextBox.SetHeight(0, 1f);
        _numericTextBox.SetMargin(4f);
        _numericTextBox.Join(box);
    }

    private void AddSlideBox(UIElementGroup box)
    {
        _slideBox = new SUISlideBox();
        _slideBox.ColorMethod = _colorLerpMethod;
        _slideBox.ValueChangeCallback += () =>
        {
            double value = Min + (Max - Min) * _slideBox.Value;
            if (Increment is double d && d != 0)
            {
                value = Math.Round(value / d) * d;
            }
            value = Math.Clamp(value, Min, Max);
            SetConfigValue(value, broadcast: false);
        };
        _slideBox.EndDraggingCallback += () =>
        {
            double value = Min + (Max - Min) * _slideBox.Value;
            if (Increment is double d && d != 0)
            {
                value = Math.Round(value / d) * d;
            }
            value = Math.Clamp(value, Min, Max);
            SetConfigValue(value, broadcast: true);
        };
        _slideBox.SetMargin(4f);
        _slideBox.Margin = new(4f);
        _slideBox.Join(box);
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);
        if (Increment == null)
            _splitButton?.Remove();
        _splitButton.IgnoreMouseInteraction = !Interactable;
        _slideBox.IgnoreMouseInteraction = !Interactable;
        _numericTextBox.IgnoreMouseInteraction = !Interactable;

        // 简直是天才的转换
        var value = float.Parse(GetValue()!.ToString()!);
        if (!_numericTextBox.IsFocus)
            _numericTextBox.Value = value;
        _slideBox.Value = Utils.GetLerpValue((float)Min, (float)Max, value);
    }

    public float LerpValue
    {
        get => _slideBox.Value;
        set
        {
            if (!_numericTextBox.IsFocus)
                _numericTextBox.Value = MathHelper.Lerp((float)Min, (float)Max, value);
            _slideBox.Value = value;
        }
    }

    public void OutSideEditEnd() => _slideBox?.OutSideEditEnd();
}