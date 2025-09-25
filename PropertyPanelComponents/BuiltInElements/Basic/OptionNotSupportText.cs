using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.Localization;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;

public class OptionNotSupportText : PropertyOption
{
    protected override void FillOption()
    {
        UITextView notSupportText = new()
        {
            Text = Language.GetTextValue("Mods.PropertyPanelLibrary.OptionNotSupportText"),
            TextAlign = new Vector2(1, 0.5f)
        };
        notSupportText.Join(this);
    }

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegisterOptionToType(this, typeof(nint));
        PropertyOptionSystem.RegisterOptionToType(this, typeof(nuint));
        PropertyOptionSystem.RegistreOptionToTypeComplex(this, type => type.IsSubclassOf(typeof(Delegate)));
    }
}