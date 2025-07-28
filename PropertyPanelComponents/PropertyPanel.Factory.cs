using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;

namespace PropertyPanelLibrary.PropertyPanelComponents;

public partial class PropertyPanel
{
    public static PropertyPanel FromObject(object target)
    {
        var panel = new PropertyPanel();
        panel.SetWidth(0, 1);
        panel.SetHeight(0, 1);
        panel.Filler = new ObjectMetaDataFiller(target);
        return panel;
    }
}