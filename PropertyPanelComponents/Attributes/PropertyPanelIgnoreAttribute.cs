using System;

namespace PropertyPanelLibrary.PropertyPanelComponents.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class PropertyPanelIgnoreAttribute : Attribute
{
}