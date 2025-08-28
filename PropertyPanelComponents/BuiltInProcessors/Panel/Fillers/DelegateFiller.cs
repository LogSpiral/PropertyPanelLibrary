using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;

public class DelegateFiller : IPropertyOptionFiller
{
    public delegate void FillingOptionListDelegate(List<PropertyOption> list);

    public event FillingOptionListDelegate OnFillingOptionList;

    void IPropertyOptionFiller.FillingOptionList(List<PropertyOption> list) => OnFillingOptionList?.Invoke(list);
}