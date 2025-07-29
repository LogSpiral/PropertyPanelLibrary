using PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;

public class ObjectMetaDataFiller(object configObject) : IPropertyOptionFiller
{

#nullable enable
    PropertyOption? Owner { get; set; }
    object? Item { get; set; }
    PropertyFieldWrapper? VariableInfo { get; set; }
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

    private static PropertyOption ListIndexToOption(IList list, int index, object item = null, PropertyFieldWrapper variableInfo = null)
    {
        var option = PropertyOptionSystem.GenerateOptionElement(list, index, item, variableInfo);
        try
        {
            option.Bind(list, index, item, variableInfo);
        }
        catch
        {
            option = new OptionNotSupportText();
            option.Bind(list, index, item, variableInfo);
        }
        return option;
    }

    public ObjectMetaDataFiller SetAsSubOption(PropertyOption owner, object item, PropertyFieldWrapper variableInfo) 
    {
        Owner = owner;
        Item = item;
        VariableInfo = variableInfo;
        return this;
    }

    void IPropertyOptionFiller.FillingOptionList(List<PropertyOption> list)
    {
        if (configObject is IList objectList)
        {
            int index = 0;
            foreach (var item in objectList)
            {
                var option = ListIndexToOption(objectList, index++, Item, VariableInfo);
                option.owner = Owner;
                list.Add(option);
            }
        }
        else
        {
            var variableInfos = ConfigManager.GetFieldsAndProperties(configObject);
            foreach (var variableInfo in variableInfos)
            {
                var option = VariableInfoToOption(configObject, variableInfo);
                option.owner = Owner;
                list.Add(option);
            }
        }

    }
}