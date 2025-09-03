using System;

namespace PropertyPanelLibrary.PropertyPanelComponents.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class LabelPresentValueAttribute(bool isPresent) : Attribute
{
    public bool IsPresent { get; set; } = isPresent;

    public LabelPresentValueAttribute() : this(true)
    {
    }
}