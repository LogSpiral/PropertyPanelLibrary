using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using System.Collections.Generic;
using System.Linq;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.InteractableHandlers;

public class CombinedInteractableHandler(params IEnumerable<IPropertyOptionInteractableHandler> interactableHandlers) : IPropertyOptionInteractableHandler
{
    public CombiningMode CombiningMode { get; set; } = CombiningMode.And;

    public IPropertyOptionInteractableHandler Clone() => new CombinedInteractableHandler(from interactableHandler in interactableHandlers select interactableHandler.Clone());

    bool IPropertyOptionInteractableHandler.CheckInteractable(PropertyOption option, out string reason)
    {
        switch (CombiningMode)
        {
            case CombiningMode.And:
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

            case CombiningMode.Or:
                foreach (var interactableHandler in interactableHandlers)
                {
                    if (interactableHandler.CheckInteractable(option, out string message))
                    {
                        reason = string.Empty;
                        return true;
                    }
                }
                reason = "Not Pass Any Check"; // TODO 本地化
                return false;

            case CombiningMode.Xor:
                int counter = 0;
                foreach (var interactableHandler in interactableHandlers)
                    if (interactableHandler.CheckInteractable(option, out string message))
                        counter++;
                if (counter % 2 == 1)
                {
                    reason = ""; // TODO 本地化
                    return true;
                }
                else
                {
                    reason = "Not Pass Xor Check"; // TODO 本地化
                    return false;
                }

            default:
                goto case CombiningMode.And;
        }
    }
}