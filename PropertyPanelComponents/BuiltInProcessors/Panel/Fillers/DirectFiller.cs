using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;

public class DirectFiller(params IEnumerable<PropertyOption> options) : IPropertyOptionFiller
{
    private readonly IEnumerable<PropertyOption> _options = options;

    void IPropertyOptionFiller.FillingOptionList(List<PropertyOption> list)
    {
        foreach (var option in _options)
            list.Add(option);
    }
}