using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System;
using Terraria.ModLoader;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;

public class OptionNotSupportText : PropertyOption
{
    protected override void FillOption()
    {
        UITextView notSupportText = new();
        notSupportText.Text = "不支持喵！";
        notSupportText.SetLeft(0, 0, 1);
        notSupportText.SetTop(0, 0, 0.5f);
        notSupportText.TextAlign = new(1, 0.5f);
        notSupportText.Join(this);
    }

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegisterOptionToType(this, typeof(nint));
        PropertyOptionSystem.RegisterOptionToType(this, typeof(nuint));
        PropertyOptionSystem.RegistreOptionToTypeComplex(this, type => type.IsSubclassOf(typeof(Delegate)));
    }
}