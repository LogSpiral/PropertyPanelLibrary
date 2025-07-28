using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.OptionDecorators;

/// <summary>
/// 组合装饰器
/// </summary>
/// <param name="decorators"></param>
public class CombinedOptionDecorator(params IEnumerable<IPropertyOptionDecorator> decorators) : IPropertyOptionDecorator
{
    void IPropertyOptionDecorator.PostFillOption(PropertyOption option)
    {
        foreach(var decorator in decorators)
            decorator.PostFillOption(option);
    }

    void IPropertyOptionDecorator.PreFillOption(PropertyOption option)
    {
        foreach (var decorator in decorators)
            decorator.PreFillOption(option);
    }

    void IPropertyOptionDecorator.UnloadDecorate(PropertyOption option)
    {
        foreach (var decorator in decorators)
            decorator.UnloadDecorate(option);
    }
}