using PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework;
using SilkyUIFramework.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.Core;

public class PropertyOptionSystem : ModSystem
{
    #region Register

    private static readonly HashSet<PropertyOption> _registeredOptions = [];
    private static readonly Dictionary<Type, Type> _simpleTypeOptionDictionary = [];
    private static readonly Dictionary<Func<Type, bool>, Type> _complexDefaultTypeOptionDictionary = [];

    public static void RegisterOption(PropertyOption option) => _registeredOptions.Add(option);

    public static void RegisterOptionToType(PropertyOption option, Type variableType) => _simpleTypeOptionDictionary.TryAdd(variableType, option.GetType());

    public static void RegistreOptionToTypeComplex(PropertyOption option, Func<Type, bool> func) => _complexDefaultTypeOptionDictionary[func] = option.GetType();

    public override void Unload()
    {
        _simpleTypeOptionDictionary.Clear();
        _complexDefaultTypeOptionDictionary.Clear();
        _registeredOptions.Clear();
    }

    #endregion Register


    public static PropertyOption GenerateOptionElement(object configObject, PropertyFieldWrapper variableInfo)
    {
        return Internal_GenerateOptionElement(
            variableInfo.Type, 
            ConfigManager.GetCustomAttributeFromMemberThenMemberType<CustomOptionElementAttribute>(
                variableInfo, 
                configObject, 
                null));
    }
    public static PropertyOption GenerateOptionElement(IList list, int index, object? item = null, PropertyFieldWrapper? variableInfo = null) 
    {
        return Internal_GenerateOptionElement(
            list[index].GetType(), 
            item == null ? null 
            : ConfigManager.GetCustomAttributeFromMemberThenMemberType<CustomOptionElementAttribute>(
                variableInfo, 
                item, 
                list));
    }
    static PropertyOption Internal_GenerateOptionElement(Type variableType, CustomOptionElementAttribute customOptionAttribute = null)
    {
        Type optionType = typeof(OptionNotSupportText);
        if (customOptionAttribute != null)
            optionType = customOptionAttribute.Type;
        else if (_simpleTypeOptionDictionary.TryGetValue(variableType, out var resultOptionType))
            optionType = resultOptionType;
        else
            foreach (var pair in _complexDefaultTypeOptionDictionary)
                if (pair.Key.Invoke(variableType))
                {
                    optionType = pair.Value;
                    break;
                }
        return Activator.CreateInstance(optionType) as PropertyOption;
    }
}