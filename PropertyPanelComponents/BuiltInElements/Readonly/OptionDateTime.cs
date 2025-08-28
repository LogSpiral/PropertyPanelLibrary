using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Readonly;

public class OptionDateTime : PropertyOption
{
    protected override void Register(Mod mod) => PropertyOptionSystem.RegisterOptionToType(this, typeof(DateTime));
    private UITextView DateText { get; set; }
    private UIElementGroup Mask { get; set; }
    protected override void FillOption()
    {
        Mask = new()
        {
            FlexGrow = 1,
            FlexShrink = 1,
            FitHeight = true,
            FitWidth = true,
            Padding = new(0,-4,8,4),
            Left = new(0,0,1)
        };
        Mask.Join(this);
        DateText = new()
        {
            Top = new(0, 0, .5f),
            Left = new(0, 0, 1),
            TextAlign = new(1, .5f),
            Text = ((DateTime)GetValue()).ToString("yyyy-MM-dd HH:mm:ss")
        };
        DateText.Join(Mask);
    }
}
