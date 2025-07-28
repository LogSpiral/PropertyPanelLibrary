using PropertyPanelLibrary.PropertyPanelComponents.Core;
using System;

namespace PropertyPanelLibrary.PropertyPanelComponents.Core;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field)]
public class CustomOptionElementAttribute : Attribute
{
    public Type Type { get; init; }

    public CustomOptionElementAttribute(Type optionType)
    {
        if (!optionType.IsAssignableTo(typeof(PropertyOption)))
            throw new ArgumentException("optionType is not PropertyOption");
        Type = optionType;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field)]
public class CustomOptionElementAttribute<T>() : CustomOptionElementAttribute(typeof(T)) where T : PropertyOption
{
}