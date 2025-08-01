using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using PropertyPanelLibrary.PropertyPanelComponents.Core;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.InteractableHandlers;

public class NoneInteractableHandler : IPropertyOptionInteractableHandler
{
    public static NoneInteractableHandler Instance { get; } = new();

    bool IPropertyOptionInteractableHandler.CheckInteractable(PropertyOption option, out string reason)
    {
        reason = string.Empty;
        return true;
    }

    IPropertyOptionInteractableHandler IPropertyOptionInteractableHandler.Clone() => Instance;
}
