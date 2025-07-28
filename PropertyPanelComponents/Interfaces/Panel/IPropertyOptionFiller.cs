using PropertyPanelLibrary.PropertyPanelComponents.Core;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;

/// <summary>
/// 填充器：计算需要加入面板的元素
/// </summary>
public interface IPropertyOptionFiller
{
    /// <summary>
    /// 将选项填充进list
    /// </summary>
    /// <param name="list"></param>
    void FillingOptionList(List<PropertyOption> list);
}