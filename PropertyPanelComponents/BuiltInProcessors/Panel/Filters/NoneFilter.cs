using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using PropertyPanelLibrary.PropertyPanelComponents.Core;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Filters;

/// <summary>
/// 不进行任何筛选的筛选器
/// </summary>
public class NoneFilter : IPropertyOptionFilter
{
    public static NoneFilter Instance { get; } = new();

    bool IPropertyOptionFilter.CheckPassFilter(PropertyOption option) => true;
}