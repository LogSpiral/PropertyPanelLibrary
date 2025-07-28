using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.InteractableHandlers;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.MouseHandlers;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.OptionDecorators;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.Writers;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Decorators;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Filters;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Sorters;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;

namespace PropertyPanelLibrary.PropertyPanelComponents.Interfaces;

public static class PropertyPanelProcessorExtensions
{
    public static CombinedFiller Add(this IPropertyOptionFiller self, IPropertyOptionFiller filler) 
    {
        return new CombinedFiller(self, filler);
    }

    public static CombinedFilter Add(this IPropertyOptionFilter self, IPropertyOptionFilter filter) 
    {
        return new CombinedFilter(self, filter);
    }

    public static CombinedSorter Add(this IPropertyOptionSorter self, IPropertyOptionSorter sorter) 
    {
        return new CombinedSorter(self, sorter);
    }

    public static CombinedDecorator Add(this IPropertyPanelDecorator self, IPropertyPanelDecorator decorator) 
    {
        return new CombinedDecorator(self, decorator);
    }

    public static CombinedWriter Add(this IPropertyValueWriter self, IPropertyValueWriter writer) 
    {
        return new CombinedWriter(self, writer);
    }

    public static CombinedMouseHandler Add(this IPropertyMouseHandler self, IPropertyMouseHandler mouseHandler) 
    {
        return new CombinedMouseHandler(self, mouseHandler);
    }

    public static CombinedInteractableHandler Add(this IPropertyOptionInteractableHandler self, IPropertyOptionInteractableHandler interactableHandler) 
    {
        return new CombinedInteractableHandler(self, interactableHandler);
    }

    public static CombinedOptionDecorator Add(this IPropertyOptionDecorator self, IPropertyOptionDecorator decorator)
    {
        return new CombinedOptionDecorator(self, decorator);
    }
}
