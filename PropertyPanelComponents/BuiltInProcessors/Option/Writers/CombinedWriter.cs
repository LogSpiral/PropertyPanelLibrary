using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using System.Collections.Generic;
using System.Linq;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.Writers;

public class CombinedWriter(params IEnumerable<IPropertyValueWriter> writers) : IPropertyValueWriter
{
    IPropertyValueWriter IPropertyValueWriter.Clone() => new CombinedWriter(from writer in writers select writer.Clone());

    void IPropertyValueWriter.WriteValue(PropertyOption option, object value, bool broadCast)
    {
        foreach (var writer in writers)
            writer.WriteValue(option, value, broadCast);
    }
}