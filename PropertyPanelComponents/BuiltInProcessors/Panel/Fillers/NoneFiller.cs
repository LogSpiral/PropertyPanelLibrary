using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;

public class NoneFiller : IPropertyOptionFiller
{
    public static NoneFiller Instance { get; } = new();

    void IPropertyOptionFiller.FillingOptionList(List<PropertyOption> list)
    {
    }
}