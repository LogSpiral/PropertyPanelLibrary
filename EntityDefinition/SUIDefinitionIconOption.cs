using SilkyUIFramework.Extensions;
using Microsoft.Xna.Framework;
using SilkyUIFramework;
using EDefinition = Terraria.ModLoader.Config.EntityDefinition;
using SilkyUIFramework.Elements;
namespace PropertyPanelLibrary.EntityDefinition;

public class SUIDefinitionIconOption : SUIEntityDefinitionOption
{
    public override void OnSetDefinition(EDefinition current, EDefinition previous)
    {
        base.OnSetDefinition(current, previous);
        Tooltip = current.DisplayName;
    }
    protected SUIImage Icon { get; set; }

    public SUIDefinitionIconOption()
    {
        Padding = new Margin(4);
        BorderRadius = new Vector4(2);
        FitWidth = true;
        FitHeight = true;
        BackgroundColor = Color.Black * .1f;
        Icon = new SUIImage
        {
            FitWidth = true,
            FitHeight = true
        };
        Icon.Join(this);
    }

}
