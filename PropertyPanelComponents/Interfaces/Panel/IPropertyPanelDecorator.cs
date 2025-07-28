namespace PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;

/// <summary>
/// 装饰器：对面板本身添加新的控件来辅助行为
/// </summary>
public interface IPropertyPanelDecorator
{
    /// <summary>
    /// 填充属性面板前的装饰
    /// </summary>
    /// <param name="panel"></param>
    void PreFillPanel(PropertyPanel panel);

    /// <summary>
    /// 填充属性面板后的装饰
    /// </summary>
    /// <param name="panel"></param>
    void PostFillPanel(PropertyPanel panel);

    /// <summary>
    /// 卸载装饰
    /// </summary>
    /// <param name="panel"></param>
    void UnloadDecorate(PropertyPanel panel);
}