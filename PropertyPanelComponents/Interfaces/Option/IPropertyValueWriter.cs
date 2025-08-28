using PropertyPanelLibrary.PropertyPanelComponents.Core;

namespace PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;

/// <summary>
/// 写入器：控制写入值时的行为
/// </summary>
public interface IPropertyValueWriter
{
    /// <summary>
    /// 写入逻辑
    /// </summary>
    /// <param name="option">选项元素，提供元数据等</param>
    /// <param name="value">当前写入的值</param>
    /// <param name="broadCast">是否正式写入</param>
    void WriteValue(PropertyOption option, object value, bool broadCast);

    /// <summary>
    /// 用于非单例模式的选项处理器
    /// </summary>
    /// <returns></returns>
    IPropertyValueWriter Clone();
}