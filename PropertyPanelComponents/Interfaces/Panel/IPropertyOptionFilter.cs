using PropertyPanelLibrary.PropertyPanelComponents.Core;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;

/// <summary>
/// 筛选器：在填充基础上筛选出应当显示的部分
/// </summary>
public interface IPropertyOptionFilter
{
    /// <summary>
    /// 筛选需要最终添加到面板上的选项，默认需要用到<see cref="CheckPassFilter(PropertyOption)"/>
    /// </summary>
    /// <param name="totalList"></param>
    /// <param name="resultList"></param>
    void FliteringOptionList(IReadOnlyList<PropertyOption> totalList, List<PropertyOption> resultList)
    {
        foreach (var option in totalList)
            if (CheckPassFilter(option))
                resultList.Add(option);
    }

    /// <summary>
    /// 检测当前选项是否应当添加到面板
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    bool CheckPassFilter(PropertyOption option);
}