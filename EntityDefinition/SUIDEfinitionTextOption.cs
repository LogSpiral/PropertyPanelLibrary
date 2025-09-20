using SilkyUIFramework.Extensions;
using Microsoft.Xna.Framework;
using SilkyUIFramework;
using EDefinition = Terraria.ModLoader.Config.EntityDefinition;
using SilkyUIFramework.Elements;
namespace PropertyPanelLibrary.EntityDefinition;

public class SUIDEfinitionTextOption : SUIEntityDefinitionOption
{
    public SUIDEfinitionTextOption()
    {
        Padding = new Margin(4);
        BorderRadius = new Vector4(2);
        FitWidth = true;
        FitHeight = true;
        BackgroundColor = Color.Black * .1f;
        NameText = new UITextView
        {
            TextAlign = new Vector2(.5f),
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