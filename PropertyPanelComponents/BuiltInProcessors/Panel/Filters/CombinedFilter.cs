using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Filters;

/// <summary>
/// 组合筛选器，需要通过每一个筛选器才行
/// </summary>
public class CombinedFilter(params IEnumerable<IPropertyOptionFilter> filters) : IPropertyOptionFilter
{
    // TODO 增加或模式筛选和异或模式筛选
    bool IPropertyOptionFilter.CheckPassFilter(PropertyOption option)
    {
        foreach (var filter in filters)
            if (!filter.CheckPassFilter(option))
                return false;
        return true;
    }
}