using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using SilkyUIFramework;
using System.Collections.Generic;
using System.Linq;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.MouseHandlers;

public class CombinedMouseHandler(params IEnumerable<IPropertyMouseHandler> mouseHandlers) : IPropertyMouseHandler
{
    IPropertyMouseHandler IPropertyMouseHandler.Clone() => new CombinedMouseHandler(from mouseHandler in mouseHandlers select mouseHandler.Clone());

    void IPropertyMouseHandler.LeftMouseClick(UIMouseEvent evt, PropertyOption option)
    {
        foreach (var mouseHandler in mouseHandlers)
            mouseHandler.LeftMouseClick(evt, option);
    }

    void IPropertyMouseHandler.MiddleMouseClick(UIMouseEvent evt, PropertyOption option)
    {
        foreach (var mouseHandler in mouseHandlers)
            mouseHandler.MiddleMouseClick(evt, option);
    }

    void IPropertyMouseHandler.RightMouseClick(UIMouseEvent evt, PropertyOption option)
    {
        foreach (var mouseHandler in mouseHandlers)
            mouseHandler.RightMouseClick(evt, option);
    }
}