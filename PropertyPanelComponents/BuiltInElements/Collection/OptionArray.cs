using PropertyPanelLibrary.PropertyPanelComponents.Core;
using System;
using Terraria.ModLoader;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Collection;

public class OptionArray:OptionCollection
{
    protected override bool CanItemBeAdded => false;

    protected override void AddItem()
    {
        throw new NotImplementedException();
    }

    protected override void ClearCollection()
    {
        throw new NotImplementedException();
    }


    protected override void PrepareTypes()
    {

    }

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegistreOptionToTypeComplex(this, type => type.IsArray);
    }
    protected override bool ShouldAppendDeleteButton() => NullAllowedAttribute != null;
}
