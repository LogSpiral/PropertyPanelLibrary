using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.OptionDecorators;

public class DelegateOptionDecorator : IPropertyOptionDecorator
{
    public delegate void DecorateOptionDelegate(PropertyOption option);

    public event DecorateOptionDelegate OnPreFillOption;

    public event DecorateOptionDelegate OnPostFillOption;

    public event DecorateOptionDelegate OnUnloadDecorate;

    void IPropertyOptionDecorator.PostFillOption(PropertyOption option) => OnPostFillOption?.Invoke(option);

    void IPropertyOptionDecorator.PreFillOption(PropertyOption option) => OnPreFillOption?.Invoke(option);

    void IPropertyOptionDecorator.UnloadDecorate(PropertyOption option) => OnUnloadDecorate?.Invoke(option);

    IPropertyOptionDecorator IPropertyOptionDecorator.Clone() => this;
}