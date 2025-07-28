using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using SilkyUIFramework.Extensions;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Decorators;

public class FitHeightDecorator : IPropertyPanelDecorator
{
    public static FitHeightDecorator Instance { get; } = new();
    void IPropertyPanelDecorator.PostFillPanel(PropertyPanel panel)
    {
    }

    void IPropertyPanelDecorator.PreFillPanel(PropertyPanel panel)
    {
        panel.FitHeight = true;
        panel.OptionList.FitHeight = true;
        panel.OptionList.Mask.FitHeight = true;
        panel.OptionList.Container.FitHeight = true;
        panel.OptionList.ScrollBar.Remove();
    }

    void IPropertyPanelDecorator.UnloadDecorate(PropertyPanel panel)
    {
        panel.FitHeight = false;
        panel.OptionList.FitHeight = false;
        panel.OptionList.Mask.FitHeight = false;
        panel.OptionList.Container.FitHeight = false;
        panel.OptionList.ScrollBar.Join(panel.OptionList);
    }
}
