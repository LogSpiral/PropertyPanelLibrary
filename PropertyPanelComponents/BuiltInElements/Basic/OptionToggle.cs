using PropertyPanelLibrary.BasicElements;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework.Extensions;
using Terraria;
using Terraria.ModLoader;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;

public class OptionToggle : PropertyOption
{
    protected override void FillOption()
    {
        SUIToggle toggle = new SUIToggle();
        toggle.SetSize(48, 26);
        toggle.Join(this);
        toggle.Enabled = (bool)GetValue();
        toggle.OnContentsChanged += (sender, arg) =>
        {
            SetValue(arg.NewValue);
        };
        toggle.OnUpdateStatus += delegate
        {
            toggle.SyncState((bool)GetValue());
        };
        base.FillOption();
    }

    protected override void Register(Mod mod) => PropertyOptionSystem.RegisterOptionToType(this, typeof(bool));
}