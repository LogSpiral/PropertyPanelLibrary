using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;

/// <summary>
///  组合填充器
/// </summary>
/// <param name="fillers"></param>
public class CombinedFiller(params IEnumerable<IPropertyOptionFiller> fillers) : IPropertyOptionFiller
{
    void IPropertyOptionFiller.FillingOptionList(List<PropertyOption> list)
    {
        foreach (var filler in fillers)
            filler.FillingOptionList(list);
    }
}