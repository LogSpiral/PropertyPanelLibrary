using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Decorators;

/// <summary>
/// 组合装饰器
/// </summary>
/// <param name="decorators"></param>
public class CombinedDecorator(params IEnumerable<IPropertyPanelDecorator> decorators) : IPropertyPanelDecorator
{
    void IPropertyPanelDecorator.PreFillPanel(PropertyPanel panel)
    {
        foreach (var decorator in decorators)
            decorator.PreFillPanel(panel);
    }

    void IPropertyPanelDecorator.PostFillPanel(PropertyPanel panel)
    {
        foreach (var decorator in decorators)
            decorator.PostFillPanel(panel);
    }

    void IPropertyPanelDecorator.UnloadDecorate(PropertyPanel panel)
    {
        foreach (var decorator in decorators)
            decorator.UnloadDecorate(panel);
    }
}