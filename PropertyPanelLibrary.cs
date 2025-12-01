using SilkyUIFramework;
using System.Reflection;
using Terraria.ModLoader;

namespace PropertyPanelLibrary;

public class PropertyPanelLibrary : Mod
{
    public static MethodInfo SetFocusMethod { get; private set; }
    public static PropertyInfo MouseElementProperty { get; private set; }
    public override void Load()
    {
        SetFocusMethod = typeof(SilkyUI).GetMethod("SetFocus",BindingFlags.Instance | BindingFlags.NonPublic);
        MouseElementProperty = typeof(SilkyUI).GetProperty("MouseElement", BindingFlags.Instance | BindingFlags.NonPublic);
        base.Load();
    }
}