using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Filters;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Sorters;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Decorators;

/// <summary>
/// 添加搜索条在面板最上方
/// </summary>
public class SearchBarDecorator : IPropertyPanelDecorator
{
    void IPropertyPanelDecorator.PreFillPanel(PropertyPanel panel)
    {
        // TODO 添加搜索栏
    }

    void IPropertyPanelDecorator.PostFillPanel(PropertyPanel panel)
    {
    }

    void IPropertyPanelDecorator.UnloadDecorate(PropertyPanel panel)
    {
    }

    private SearchBarDecorator()
    {
    }

    public static SearchBarDecorator NewSearchBar(PropertyPanel panel, out SearchingTextFilter filter, out SearchingTextSorter sorter)
    {
        // TODO 完成文本信息绑定
        filter = new SearchingTextFilter();
        sorter = new SearchingTextSorter();
        return new SearchBarDecorator();
    }
}