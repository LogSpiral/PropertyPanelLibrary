using System;
using System.Linq;

namespace PropertyPanelLibrary.BasicElements;

public class SUINumericTextBox : SUIEditTextBox
{
    public SUINumericTextBox()
    {
        InnerText.ContentChanging += (sender, args) => new string([.. args.NewText.Where(c => char.IsDigit(c) || c is '.' or '-' or ',')]);
        InnerText.EndTakingInput += (sender, args) =>
        {
            Value = !double.TryParse(args.NewValue, out double digit)
                ? DefaultValue
                : Math.Clamp(digit, MinValue, MaxValue);
        };
    }

    public string Format = "0.00";
    public double DefaultValue = 0;
    public double MinValue = 0;
    public double MaxValue = 1;

    public string Text
    {
        get => InnerText.Text;
        set => InnerText.Text = value;
    }

    public bool IsValueSafe => double.TryParse(Text, out _);

    public double Value
    {
        get => double.Parse(Text);
        set => Text = value.ToString(Format);
    }
}