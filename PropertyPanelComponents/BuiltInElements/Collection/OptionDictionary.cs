using Newtonsoft.Json;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Collection;

public class OptionDictionary : OptionCollection
{
    internal Type keyType;
    internal Type valueType;
    public List<IDictionaryElementWrapper> dataWrapperList;

    // These 2 hold the default value of the dictionary value, hence ValueValue
    protected DefaultDictionaryKeyValueAttribute defaultDictionaryKeyValueAttribute;

    protected JsonDefaultDictionaryKeyValueAttribute jsonDefaultDictionaryKeyValueAttribute;
    protected override bool CanItemBeAdded => true;
    private PropertyFieldWrapper wrappermemberInfo;

    protected override void AddItem()
    {
        object keyValue;

        if (defaultDictionaryKeyValueAttribute != null)
        {
            keyValue = defaultDictionaryKeyValueAttribute.Value;
        }
        else
        {
            keyValue = ConfigManager.AlternateCreateInstance(keyType);

            if (!keyType.IsValueType && keyType != typeof(string))
            {
                string json = jsonDefaultDictionaryKeyValueAttribute?.Json ?? "{}";

                JsonConvert.PopulateObject(json, keyValue, ConfigManager.serializerSettings);
            }
        }
        var dict = (IDictionary)Data;
        if (!dict.Contains(keyValue))
            dict.Add(keyValue, CreateCollectionElementInstance(valueType));
    }

    protected override void ClearCollection()
    {
        ((IDictionary)Data).Clear();
    }

    protected override void PrepareTypes()
    {
        var variableInfo = MetaData.VariableInfo;
        keyType = variableInfo.Type.GetGenericArguments()[0];
        valueType = variableInfo.Type.GetGenericArguments()[1];
        JsonDefaultListValueAttribute = ConfigManager.GetCustomAttributeFromCollectionMemberThenElementType<JsonDefaultListValueAttribute>(variableInfo.MemberInfo, valueType);
        defaultDictionaryKeyValueAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<DefaultDictionaryKeyValueAttribute>(variableInfo, null, null);
        jsonDefaultDictionaryKeyValueAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<JsonDefaultDictionaryKeyValueAttribute>(variableInfo, null, null);
    }

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegistreOptionToTypeComplex(this, type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>));
    }

    protected override IPropertyOptionFiller GetInternalPanelFiller(object data)
    {
        dataWrapperList = [];
        Type genericType = typeof(DictionaryElementWrapper<,>).MakeGenericType(keyType, valueType);

        var keys = ((IDictionary)Data).Keys;
        var values = ((IDictionary)Data).Values;
        var keysEnumerator = keys.GetEnumerator();
        var valuesEnumerator = values.GetEnumerator();
        int i = 0;

        while (keysEnumerator.MoveNext())
        {
            valuesEnumerator.MoveNext();
            var proxy = (IDictionaryElementWrapper)Activator.CreateInstance(genericType, [keysEnumerator.Current, valuesEnumerator.Current, (IDictionary)Data]);
            dataWrapperList.Add(proxy);
            wrappermemberInfo ??= ConfigManager.GetFieldsAndProperties(this).ToList()[0];

            i++;
        }

        return base.GetInternalPanelFiller(dataWrapperList);
    }
}