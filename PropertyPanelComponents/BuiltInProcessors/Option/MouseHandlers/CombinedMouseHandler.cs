using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.MouseHandlers;

public class CombinedMouseHandler(params IEnumerable<IPropertyMouseHandler> mouseHandlers) : IPropertyMouseHandler
{
    void IPropertyMouseHandler.LeftMouseClick(UIMouseEvent evt, PropertyOption option)
    {
        foreach (var handlers in mouseHandlers)
            handlers.LeftMouseClick(evt, option);
    }

    void IPropertyMouseHandler.MiddleMouseClick(UIMouseEvent evt, PropertyOption option)
    {
        foreach (var handlers in mouseHandlers)
            handlers.MiddleMouseClick(evt, option);
    }

    void IPropertyMouseHandler.RightMouseClick(UIMouseEvent evt, PropertyOption option)
    {
        foreach (var handlers in mouseHandlers)
            handlers.RightMouseClick(evt, option);
    }
}