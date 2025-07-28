using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.Writers;

public class CombinedWriter(params IEnumerable<IPropertyValueWriter> writers) : IPropertyValueWriter
{
    void IPropertyValueWriter.WriteValue(PropertyOption option, object value, bool broadCast)
    {
        foreach (var writer in writers)
            writer.WriteValue(option, value, broadCast);
    }
}