using SilkyUIFramework;
using System.Reflection;
using Terraria.ModLoader;

namespace PropertyPanelLibrary;

public class PropertyPanelLibrary : Mod
{
    public static MethodInfo UpdateFocusedElementMethod { get; private set; }
    public static PropertyInfo PressedElementsProperty { get; private set; }

    public static FieldInfo _inputStateField { get; private set; }
    public override void Load()
    {
        UpdateFocusedElementMethod = typeof(SilkyUI).GetMethod("UpdateFocusedElement", BindingFlags.Instance | BindingFlags.NonPublic);
        PressedElementsProperty = typeof(SilkyUIInputState).GetProperty("PressedElements", BindingFlags.Instance | BindingFlags.NonPublic);
        _inputStateField = typeof(SilkyUIManager).GetField("_inputState", BindingFlags.Instance | BindingFlags.NonPublic);

        base.Load();
    }
}