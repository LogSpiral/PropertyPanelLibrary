using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using Terraria;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Sorters;

public class SearchingTextSorter : IPropertyOptionSorter
{
    int IPropertyOptionSorter.EvaluatePriority(PropertyOption option) => option.Label.Contains(MatchingText).ToInt();

    public string MatchingText { get; set; }
}