using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using PropertyPanelLibrary.PropertyPanelComponents.Core;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.InteractableHandlers;

public class DelegateInteractableHandler : IPropertyOptionInteractableHandler
{
    public delegate bool OptionInteractableDelegate(PropertyOption option, out string reason);
    public event OptionInteractableDelegate OnCheckInteractable;
    bool IPropertyOptionInteractableHandler.CheckInteractable(PropertyOption option, out string reason)
    {
        if (OnCheckInteractable != null)
            return OnCheckInteractable.Invoke(option, out reason);

        reason = string.Empty;
        return true;
    }

    public IPropertyOptionInteractableHandler Clone() => this;
}
