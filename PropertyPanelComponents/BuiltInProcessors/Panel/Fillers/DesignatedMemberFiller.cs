using PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;

public class DesignatedMemberFiller(params IEnumerable<(object configItem, HashSet<(string, HashSet<Attribute>)> variableName)> designatedMembers):IPropertyOptionFiller
{

    public DesignatedMemberFiller(params IEnumerable<(object configItem, HashSet<string> variableName)> designatedMembers) :
        this(from designatedMember in designatedMembers 
             select  (designatedMember.configItem,

             (from variable in designatedMember.variableName 
              select (variable, new HashSet<Attribute>())).ToHashSet()))
    {
        
    }

#nullable enable
    PropertyOption? Owner { get; set; }
#nullable disable
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

    public DesignatedMemberFiller SetAsSubOption(PropertyOption owner)
    {
        Owner = owner;
        return this;
    }

    public IEnumerable<(object configItem, HashSet<(string variableName,HashSet<Attribute> variableAttributes)> variableDatas)> DesignatedMembers { get; init; } = designatedMembers;

    void IPropertyOptionFiller.FillingOptionList(List<PropertyOption> list)
    {
        foreach (var (configItem, variableDatas) in DesignatedMembers) 
        {
            //var variableInfos = ConfigManager.GetFieldsAndProperties(configItem);
            //foreach (var variableInfo in variableInfos)
            //{
            //    if (!variableData.Contains(variableInfo.Name)) continue;
            //    var option = VariableInfoToOption(configItem, variableInfo);
            //    option.owner = Owner;
            //    list.Add(option);
            //}
            var itemType = configItem.GetType();
            foreach (var (variableName, variableAttributes) in variableDatas) 
            {
                FieldInfo fieldInfo = itemType.GetField(variableName);
                PropertyFieldWrapper variableInfo;
                if (fieldInfo == null)
                {
                    PropertyInfo propertyInfo = itemType.GetProperty(variableName);
                    if (propertyInfo == null)
                        continue;
                    else
                        variableInfo = new(propertyInfo);
                }
                else
                    variableInfo = new(fieldInfo);

                var option = VariableInfoToOption(configItem, variableInfo);
                option.owner = Owner;
                option.CheckDesignagedAttributes(variableAttributes);
                list.Add(option);
            }
        }

    }
}
