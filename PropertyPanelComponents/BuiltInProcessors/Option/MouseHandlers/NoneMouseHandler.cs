using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using SilkyUIFramework;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.MouseHandlers;

public class NoneMouseHandler : IPropertyMouseHandler
{
    public static NoneMouseHandler Instance { get; } = new NoneMouseHandler();

    public IPropertyMouseHandler Clone() => Instance;

    void IPropertyMouseHandler.LeftMouseClick(UIMouseEvent evt, PropertyOption option)
    {
    }

    void IPropertyMouseHandler.MiddleMouseClick(UIMouseEvent evt, PropertyOption option)
    {
    }

    void IPropertyMouseHandler.RightMouseClick(UIMouseEvent evt, PropertyOption option)
    {
    }
}