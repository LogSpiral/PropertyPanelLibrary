using Microsoft.Xna.Framework;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using System;
using System.Collections.Generic;
using Terraria;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.Writers;

public class DefaultWriter : IPropertyValueWriter
{
    public static DefaultWriter Instance { get; } = new();

    IPropertyValueWriter IPropertyValueWriter.Clone() => Instance;

    void IPropertyValueWriter.WriteValue(PropertyOption option, object value, bool broadCast)
    {
        var item = option.MetaData.Item;
        if (!option.MetaData.Item.GetType().IsValueType)
        {
            option.MetaData.SetValue(value);
        }
        else
        {
            option.MetaData.VariableInfo.SetValue(item, value);
            option.owner?.SetValue(item, broadCast);
        }
    }

    
}