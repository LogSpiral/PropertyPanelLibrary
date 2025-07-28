using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Sorters;

/// <summary>
/// 不进行任何排序
/// </summary>
public class NoneSorter : IPropertyOptionSorter
{
    public static NoneSorter Instance { get; } = new();

    void IPropertyOptionSorter.SortingOptionList(List<PropertyOption> list)
    {
    }

    int IPropertyOptionSorter.EvaluatePriority(PropertyOption option) => 0;
}