using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;
using System.Linq;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.OptionDecorators;

public class LabelOptionDecorator : IPropertyOptionDecorator
{
    private UITextView LabelText { get; set; }

    public static LabelOptionDecorator NewLabelDecorator() => new();

    void IPropertyOptionDecorator.PreFillOption(PropertyOption option)
    {
        var labelText = LabelText = new UITextView();
        labelText.Text = option.Label;
        labelText.SetTop(-4, 0, 0);
        labelText.Join(option);
        labelText.OnUpdate += delegate
        {
            labelText.Text = option.Label;
        };
    }

    void IPropertyOptionDecorator.PostFillOption(PropertyOption option)
    {
    }

    void IPropertyOptionDecorator.UnloadDecorate(PropertyOption option)
    {
        LabelText?.Remove();
    }

    IPropertyOptionDecorator IPropertyOptionDecorator.Clone() => NewLabelDecorator();
}