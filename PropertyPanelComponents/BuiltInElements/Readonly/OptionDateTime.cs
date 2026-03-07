using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System;
using Terraria.ModLoader;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Readonly;

public class OptionDateTime : PropertyOption
{
    protected override void Register(Mod mod) => PropertyOptionSystem.RegisterOptionToType(this, typeof(DateTime));
    private UITextView DateText { get; set; }
    protected override void FillOption()
    {
        DateText = new UITextView
        {
            Text = ((DateTime)GetValue()).ToString("yyyy-MM-dd HH:mm:ss")
        };
        DateText.Join(this);
    }
}
