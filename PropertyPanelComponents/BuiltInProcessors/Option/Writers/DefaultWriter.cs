using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using PropertyPanelLibrary.PropertyPanelComponents.Core;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.Writers;

public class DefaultWriter : IPropertyValueWriter
{
    public static DefaultWriter Instance { get; } = new();

    void IPropertyValueWriter.WriteValue(PropertyOption option, object value, bool broadCast)
    {
    }
}