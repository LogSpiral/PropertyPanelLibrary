using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Filters;

/// <summary>
/// 组合筛选器，需要通过每一个筛选器才行
/// </summary>
public class CombinedFilter(params IEnumerable<IPropertyOptionFilter> filters) : IPropertyOptionFilter
{
    public CombiningMode CombiningMode { get; set; } = CombiningMode.Xor;

    bool IPropertyOptionFilter.CheckPassFilter(PropertyOption option)
    {
        switch (CombiningMode)
        {
            case CombiningMode.And:
                foreach (var filter in filters)
                    if (!filter.CheckPassFilter(option))
                        return false;
                return true;

            case CombiningMode.Or:
                foreach (var filter in filters)
                    if (filter.CheckPassFilter(option))
                        return true;
                return false;

            case CombiningMode.Xor:
                int counter = 0;
                foreach (var filter in filters)
                    if (filter.CheckPassFilter(option))
                        counter++;
                return counter % 2 == 1;

            default:
                goto case CombiningMode.And;
        }
    }
}