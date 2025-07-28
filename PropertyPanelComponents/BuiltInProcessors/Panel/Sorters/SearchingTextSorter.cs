using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using Terraria;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Sorters;

public class SearchingTextSorter : IPropertyOptionSorter
{
    int IPropertyOptionSorter.EvaluatePriority(PropertyOption option) => option.Label.Contains(MatchingText).ToInt();

    public string MatchingText { get; set; }
}