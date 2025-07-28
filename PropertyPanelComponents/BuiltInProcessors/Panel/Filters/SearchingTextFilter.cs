using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using PropertyPanelLibrary.PropertyPanelComponents.Core;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Filters;

public class SearchingTextFilter : IPropertyOptionFilter
{
    public string MatchingText { get; set; }

    bool IPropertyOptionFilter.CheckPassFilter(PropertyOption option) => option.Label.Contains(MatchingText);
}