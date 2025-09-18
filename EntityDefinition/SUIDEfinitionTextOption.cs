using SilkyUIFramework.Extensions;
using Microsoft.Xna.Framework;
using EDefinition = Terraria.ModLoader.Config.EntityDefinition;
using SilkyUIFramework.Elements;
namespace PropertyPanelLibrary.EntityDefinition;

public class SUIDEfinitionTextOption : SUIEntityDefinitionOption
{
    public SUIDEfinitionTextOption()
    {
        Padding = new(4);
        BorderRadius = new(2);
        FitWidth = true;
        FitHeight = true;
        BackgroundColor = Color.Black * .1f;
        NameText = new()
        {
            TextAlign = new(.5f),
        };
        NameText.Join(this);
    }
    protected UITextView NameText { get; set; }
    public override void OnSetDefinition(EDefinition current, EDefinition previous)
    {
        base.OnSetDefinition(current, previous);
        Tooltip = current.DisplayName;
        NameText.Text = current.DisplayName;
    }
}