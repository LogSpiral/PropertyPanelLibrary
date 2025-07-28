using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Decorators;

/// <summary>
/// 不进行任何装饰的装饰器
/// </summary>
public class NoneDecorator : IPropertyPanelDecorator
{
    public static NoneDecorator Instance { get; } = new();

    void IPropertyPanelDecorator.PostFillPanel(PropertyPanel panel)
    {
    }

    void IPropertyPanelDecorator.PreFillPanel(PropertyPanel panel)
    {
    }

    void IPropertyPanelDecorator.UnloadDecorate(PropertyPanel panel)
    {
    }
}