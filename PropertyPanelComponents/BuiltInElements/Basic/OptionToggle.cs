using PropertyPanelLibrary.BasicElements;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework;
using SilkyUIFramework.Extensions;
using Terraria.ModLoader;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;

public class OptionToggle : PropertyOption
{
    protected override void FillOption()
    {
        SUIToggle toggle = new SUIToggle();
        toggle.SetSize(48, 26);
        toggle.SetLeft(-6, 0, 1);
        toggle.SetTop(0, 0, 0.5f);
        toggle.Join(this);
        toggle.Enabled = (bool)GetValue();
        toggle.OnContentsChanged += (sender, arg) =>
        {
            SetValue(arg.NewValue, true);
        };
        base.FillOption();
    }

    protected override void Register(Mod mod) => PropertyOptionSystem.RegisterOptionToType(this, typeof(bool));
}