using PropertyPanelLibrary.PropertyPanelComponents.Core;
using Terraria.ModLoader;

namespace PropertyPanelLibrary.PropertyPanelComponents.Core;

public partial class PropertyOption : ILoadable
{
    protected virtual void Register(Mod mod)
    {
    }

    public void Load(Mod mod)
    {
        PropertyOptionSystem.RegisterOption(this);
        Register(mod);
    }

    public virtual void Unload()
    {
    }
}