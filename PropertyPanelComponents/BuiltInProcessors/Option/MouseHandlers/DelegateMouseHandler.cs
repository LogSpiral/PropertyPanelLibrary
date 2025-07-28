using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework;
using SilkyUIFramework.BasicElements;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.MouseHandlers;

public class DelegateMouseHandler : IPropertyMouseHandler
{
    public MouseEventHandler OnLeftClick;
    public MouseEventHandler OnMiddleClick;
    public MouseEventHandler OnRightClick;

    void IPropertyMouseHandler.LeftMouseClick(UIMouseEvent evt, PropertyOption option) => OnLeftClick?.Invoke(option, evt);

    void IPropertyMouseHandler.MiddleMouseClick(UIMouseEvent evt, PropertyOption option) => OnMiddleClick?.Invoke(option, evt);

    void IPropertyMouseHandler.RightMouseClick(UIMouseEvent evt, PropertyOption option) => OnRightClick?.Invoke(option, evt);
}