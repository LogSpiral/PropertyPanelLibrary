using PropertyPanelLibrary.PropertyPanelComponents.Core;

namespace PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;

/// <summary>
/// 交互判定器，用于判定当前控件是否能进行交互
/// </summary>
public interface IPropertyOptionInteractableHandler
{
    bool CheckInteractable(PropertyOption option, out string reason);
}