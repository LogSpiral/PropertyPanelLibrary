using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.InteractableHandlers;

public class CombinedInteractableHandler(params IEnumerable<IPropertyOptionInteractableHandler> interactableHandlers) : IPropertyOptionInteractableHandler
{
    bool IPropertyOptionInteractableHandler.CheckInteractable(PropertyOption option, out string reason)
    {
        foreach (var interactableHandler in interactableHandlers) 
        {
            if (!interactableHandler.CheckInteractable(option, out string message)) 
            {
                reason = message;
                return false;
            }
        }
        reason = string.Empty;
        return true;
    }
}
