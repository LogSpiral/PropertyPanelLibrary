using System;

namespace PropertyPanelLibrary.EntityDefinition;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class CustomEntityDefinitionHandlerAttribute(Type type) : Attribute
{
    public Type Type { get; set; } = type;
    public IEntityDefinitionHandler Handler { get; set; } = Activator.CreateInstance(type) as IEntityDefinitionHandler;
}
public class CustomEntityDefinitionHandlerAttribute<T>() : CustomEntityDefinitionHandlerAttribute(typeof(T)) where T : IEntityDefinitionHandler;