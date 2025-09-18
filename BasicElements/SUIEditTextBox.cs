using Microsoft.Xna.Framework;
using SilkyUIFramework;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System.Linq;

namespace PropertyPanelLibrary.BasicElements;

public class SUIEditTextBox : UIElementGroup
{
    public int? MaxLength { get; set; }
    public SUIEditText InnerText { get; init; }

    public SUIEditTextBox()
    {
        var textPanel = this;
        textPanel.BackgroundColor = Color.Black * .4f;
        textPanel.BorderRadius = new(8f);
        var editText = new SUIEditText();
        editText.TextAlign = new(0f, .5f);
        editText.SetTop(-4, 0, .5f);
        editText.Join(textPanel);
        editText.SetPadding(4);
        textPanel.FitWidth = true;
        textPanel.SetMinWidth(40, 0);

        InnerText = editText;
        textPanel.GotFocus += (sender, evt) =>
        {
            if (evt.Source != sender) return;
            textPanel.SilkyUI.SetFocus(editText);
        };
        InnerText.ContentChanging += (sender, evt) =>
        {
            if (!MaxLength.HasValue || evt.NewText.Length <= MaxLength.Value) return evt.NewText;
            return evt.OldText;
        };
    }
}