using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System;
using Microsoft.Xna.Framework;
using SilkyUIFramework;
using Terraria.ModLoader;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Readonly;

public class OptionDateTime : PropertyOption
{
    protected override void Register(Mod mod) => PropertyOptionSystem.RegisterOptionToType(this, typeof(DateTime));
    private UITextView DateText { get; set; }
    private UIElementGroup Mask { get; set; }
    protected override void FillOption()
    {
        Mask = new UIElementGroup
        {
            FlexGrow = 1,
            FlexShrink = 1,
            FitHeight = true,
            FitWidth = true,
            Padding = new Margin(0,-4,8,4),
            Left = new Anchor(0,0,1)
        };
        Mask.Join(this);
        DateText = new UITextView
        {
            Top = new Anchor(0, 0, .5f),
            Left = new Anchor(0, 0, 1),
            TextAlign = new Vector2(1, .5f),
            Text = ((DateTime)GetValue()).ToString("yyyy-MM-dd HH:mm:ss")
        };
        DateText.Join(Mask);
    }
}
