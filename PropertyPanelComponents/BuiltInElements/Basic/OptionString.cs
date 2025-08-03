using Microsoft.Xna.Framework;
using PropertyPanelLibrary.BasicElements;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework;
using SilkyUIFramework.Extensions;
using Terraria.ModLoader;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;

public class OptionString : PropertyOption
{
    SUIEditTextBox TextPanel { get; set; }
    protected override void FillOption()
    {
        var textPanel = TextPanel = new SUIEditTextBox();
        textPanel.SetMinWidth(40, 0);
        textPanel.SetLeft(-10, 0, 1);
        textPanel.SetHeight(0, .75f);
        textPanel.SetTop(0, 0, 0.5f);
        textPanel.InnerText.Text = (string)GetValue() ?? string.Empty;
        textPanel.Join(this);
        textPanel.OnUpdate += delegate
        {
            if (!textPanel.InnerText.IsFocus)
                textPanel.InnerText.Text = (string)GetValue() ?? string.Empty;
        };
        textPanel.InnerText.EndTakingInput += (sender, arg) =>
        {
            SetValue(arg.NewValue);
        };
    }
    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);
        TextPanel.IgnoreMouseInteraction = !Interactable;
    }
    protected override void Register(Mod mod) => PropertyOptionSystem.RegisterOptionToType(this, typeof(string));
}