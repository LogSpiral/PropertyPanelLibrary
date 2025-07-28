using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Sorters;

/// <summary>
/// 组合排序器，先加入的排序器有更高优先级
/// </summary>
/// <param name="sorters"></param>
public class CombinedSorter(params IEnumerable<IPropertyOptionSorter> sorters) : IPropertyOptionSorter
{
    int IPropertyOptionSorter.EvaluatePriority(PropertyOption option)
    {
        int count = sorters.Count();
        int value = 0;
        foreach (var sorter in sorters)
        {
            count--;
            value += sorter.EvaluatePriority(option) << count;
        }
        return value;
    }
}