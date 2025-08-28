using SilkyUIFramework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using EDefinition = Terraria.ModLoader.Config.EntityDefinition;

namespace PropertyPanelLibrary.EntityDefinition;

public class SUIEntityDefinitionOption : UIElementGroup
{
    public required EDefinition Definition
    {
        get;
        set
        {
            SetDefinition?.Invoke(this,new(value, field));
            OnSetDefinition(value,field);
            field = value;
        }
    }
    public event EventHandler<ValueChangedEventArgs<EDefinition>> SetDefinition;
    public virtual void OnSetDefinition(EDefinition current, EDefinition previous)
    {
        Type = current?.Type ?? NullID;
        Unloaded = current?.IsUnloaded ?? false;

        if (current == null || (Type == NullID && !Unloaded))
            Tooltip = Lang.inter[23].Value; 
        else
        {
            if (Unloaded)
                Tooltip = $"{current.Name} [{current.Mod}] ({Language.GetTextValue("Mods.ModLoader.Unloaded")})";
            else
                Tooltip = $"{current.DisplayName} [{current.Mod}]";
        }
    }
	public string Tooltip { get; set; }
	public int Type { get; set; }
	protected bool Unloaded { get; set; }
	public int NullID { get; set; } = 0;
}
