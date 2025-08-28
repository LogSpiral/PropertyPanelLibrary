using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Filters;

public class SearchingTextFilter : IPropertyOptionFilter
{
    public string MatchingText { get; set; }

    bool IPropertyOptionFilter.CheckPassFilter(PropertyOption option) => option.Label.Contains(MatchingText);
}