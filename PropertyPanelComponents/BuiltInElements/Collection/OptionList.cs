using PropertyPanelLibrary.PropertyPanelComponents.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Collection;

public class OptionList : OptionCollection
{
    private Type listType;

    protected override bool CanItemBeAdded => true;
    protected override void AddItem()
    {
        ((IList)Data).Add(CreateCollectionElementInstance(listType));

    }

    protected override void ClearCollection()
    {
        ((IList)Data).Clear();
    }

    protected override void PrepareTypes()
    {
        listType = MetaData.VariableInfo.Type.GetGenericArguments()[0];
        JsonDefaultListValueAttribute = ConfigManager.GetCustomAttributeFromCollectionMemberThenElementType<JsonDefaultListValueAttribute>(MetaData.VariableInfo.MemberInfo, listType);
    }

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegistreOptionToTypeComplex(this, type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>));
    }
}
