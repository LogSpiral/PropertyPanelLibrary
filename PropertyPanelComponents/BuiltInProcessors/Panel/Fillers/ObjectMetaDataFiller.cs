using PropertyPanelLibrary.PropertyPanelComponents.Attributes;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;

public class ObjectMetaDataFiller(object configObject) : IPropertyOptionFiller
{
#nullable enable
    private PropertyOption? Owner { get; set; }
    private object? Item { get; set; }
    private PropertyFieldWrapper? VariableInfo { get; set; }
    private HashSet<Attribute>? GlobalAttributes { get; set; }
#nullable disable

    public static PropertyOption VariableInfoToOption(object configObject, PropertyFieldWrapper variableInfo)
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

    public ObjectMetaDataFiller SetGlobalAttributes(HashSet<Attribute> globalAttributes)
    {
        GlobalAttributes = globalAttributes;
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
                if (GlobalAttributes != null)
                    option.CheckDesignagedAttributes(GlobalAttributes);
                list.Add(option);
            }
        }
        else
        {
            var variableInfos = ConfigManager.GetFieldsAndProperties(configObject);
            foreach (var variableInfo in variableInfos)
            {
                if (Attribute.IsDefined(variableInfo.MemberInfo, typeof(PropertyPanelIgnoreAttribute))) continue;
                if(Attribute.IsDefined(variableInfo.MemberInfo,typeof(JsonIgnoreAttribute)))continue;
                var option = VariableInfoToOption(configObject, variableInfo);
                option.owner = Owner;
                if (GlobalAttributes != null)
                    option.CheckDesignagedAttributes(GlobalAttributes);
                list.Add(option);
            }
        }
    }
}