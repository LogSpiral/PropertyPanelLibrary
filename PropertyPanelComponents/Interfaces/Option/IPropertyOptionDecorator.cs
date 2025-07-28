using PropertyPanelLibrary.PropertyPanelComponents.Core;

namespace PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;

/// <summary>
/// 选项装饰器，给选项元素添加辅助控件等
/// </summary>
public interface IPropertyOptionDecorator
{
    /// <summary>
    /// 选项添加自身控件内容前
    /// </summary>
    /// <param name="option"></param>
    void PreFillOption(PropertyOption option);

    /// <summary>
    /// 选项添加自身控件内容后
    /// </summary>
    /// <param name="option"></param>
    void PostFillOption(PropertyOption option);

    /// <summary>
    /// 卸载装饰
    /// </summary>
    /// <param name="option"></param>
    void UnloadDecorate(PropertyOption option);
}