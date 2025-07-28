using Microsoft.Xna.Framework;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;
using System.Linq;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.OptionDecorators;

public class LabelOptionDecorator : IPropertyOptionDecorator
{
    UITextView LabelText { get; set; }

    public void PostFillOption(PropertyOption option)
    {
    }

    public void PreFillOption(PropertyOption option)
    {
        var labelText = LabelText = new UITextView();
        labelText.Text = option.Label;
        labelText.SetTop(-4, 0, 0);
        labelText.Join(option);
    }

    public void UnloadDecorate(PropertyOption option)
    {
        LabelText?.Remove();
    }

    public static LabelOptionDecorator NewLabelDecorator() => new();
}
