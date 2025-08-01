using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using PropertyPanelLibrary.PropertyPanelComponents.Core;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.Writers;

public class DelegateWriter : IPropertyValueWriter
{
    public delegate void WriteValueEvent(PropertyOption option, object value, bool broadCast);

    public event WriteValueEvent OnWriteValue;

    void IPropertyValueWriter.WriteValue(PropertyOption option, object value, bool broadCast) => OnWriteValue?.Invoke(option, value, broadCast);

    IPropertyValueWriter IPropertyValueWriter.Clone() => this;
}