﻿using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using PropertyPanelLibrary.PropertyPanelComponents.Core;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Filters;

public class DelegateFilter : IPropertyOptionFilter
{
    public delegate bool CheckPassFilterDelegate(PropertyOption option);

    public event CheckPassFilterDelegate OnCheckPassFilter;

    bool IPropertyOptionFilter.CheckPassFilter(PropertyOption option) => OnCheckPassFilter?.Invoke(option) ?? true;
}