using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Decorators;

public class DelegateDecorator : IPropertyPanelDecorator
{
    public delegate void DecoratePanelDelegate(PropertyPanel panel);

    public event DecoratePanelDelegate OnPreFillPanel;

    public event DecoratePanelDelegate OnPostFillPanel;

    public event DecoratePanelDelegate OnUnloadDecorate;

    void IPropertyPanelDecorator.PostFillPanel(PropertyPanel panel) => OnPostFillPanel?.Invoke(panel);

    void IPropertyPanelDecorator.PreFillPanel(PropertyPanel panel) => OnPreFillPanel?.Invoke(panel);

    void IPropertyPanelDecorator.UnloadDecorate(PropertyPanel panel) => OnUnloadDecorate?.Invoke(panel);
}