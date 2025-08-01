using PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using System.Collections.Generic;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;

public class DesignatedMemberFiller(params IEnumerable<(object configItem, HashSet<string> variableName)> designatedMembers):IPropertyOptionFiller
{
    private static PropertyOption VariableInfoToOption(object configObject, PropertyFieldWrapper variableInfo)
    {
        var option = PropertyOptionSystem.GenerateOptionElement(configObject, variableInfo);
        try
        {
            option.Bind(configObject, variableInfo);
        }
        catch
        {
            option = new OptionNotSupportText();
            option.Bind(configObject, variableInfo);
        }
        return option;
    }


    public IEnumerable<(object configItem, HashSet<string> variableName)> DesignatedMembers { get; init; } = designatedMembers;

    void IPropertyOptionFiller.FillingOptionList(List<PropertyOption> list)
    {
        foreach (var pair in DesignatedMembers) 
        {
            var variableInfos = ConfigManager.GetFieldsAndProperties(pair.configItem);
            foreach (var variableInfo in variableInfos)
            {
                if (!pair.variableName.Contains(variableInfo.Name)) continue;
                var option = VariableInfoToOption(pair.configItem, variableInfo);
                list.Add(option);
            }
        }

    }
}
