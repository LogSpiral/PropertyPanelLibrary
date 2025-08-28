using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Sorters;

/// <summary>
/// 使用事件的形式来自定义排序方式
/// </summary>
public class DelegateSorter : IPropertyOptionSorter
{
    public delegate int EvaluateValueDelegate(PropertyOption option);

    public event EvaluateValueDelegate OnEvaluatePriority;

    int IPropertyOptionSorter.EvaluatePriority(PropertyOption option) => OnEvaluatePriority?.Invoke(option) ?? 0;
}