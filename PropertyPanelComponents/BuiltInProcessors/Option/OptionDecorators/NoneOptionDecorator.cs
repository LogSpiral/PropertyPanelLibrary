using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using PropertyPanelLibrary.PropertyPanelComponents.Core;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.OptionDecorators;

/// <summary>
/// 不进行任何装饰的装饰器
/// </summary>
public class NoneOptionDecorator : IPropertyOptionDecorator
{
    public static NoneOptionDecorator Instance { get; } = new();

    void IPropertyOptionDecorator.PostFillOption(PropertyOption option)
    {
    }

    void IPropertyOptionDecorator.PreFillOption(PropertyOption option)
    {
    }

    void IPropertyOptionDecorator.UnloadDecorate(PropertyOption option)
    {
    }
}