using PropertyPanelLibrary.PropertyPanelComponents.Core;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;

/// <summary>
/// 排序器：在筛选基础上重新排序
/// </summary>
public interface IPropertyOptionSorter
{
    /// <summary>
    /// 对筛选列表排序的实现，默认需要用到<see cref="EvaluatePriority(PropertyOption)"/>进行比较
    /// </summary>
    /// <param name="list"></param>
    void SortingOptionList(List<PropertyOption> list) => list.Sort((x, y) => EvaluatePriority(x) - EvaluatePriority(y));

    /// <summary>
    /// 衡量当前的排序优先级
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    int EvaluatePriority(PropertyOption option);
}