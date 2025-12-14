using SilkyUIFramework;
using SilkyUIFramework.Elements;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader;

namespace PropertyPanelLibrary;

public class PropertyPanelLibrary : Mod
{
    private static MethodInfo UpdateFocusedElementMethod { get; set; }
    private static PropertyInfo PressedElementsProperty { get; set; }

    private static FieldInfo _inputStateField { get; set; }
    private static SilkyUIInputState InputState =>
            _inputStateField
            ?.GetValue(SilkyUISystem.Instance.SilkyUIManager)
            as SilkyUIInputState ?? null;
    public static Dictionary<MouseButtonType, UIView>? PressedElementsDictionary =>
            InputState is { } _inputState
            &&
            PressedElementsProperty
            ?.GetValue(_inputState)
            is Dictionary<MouseButtonType, UIView> _dictionary ? _dictionary : null;
    public static void UpdateFocusedElementCall(UIView focusedElement) 
    {
        if (InputState is not { } inputState) 
            return;
        UpdateFocusedElementMethod?.Invoke(inputState, [focusedElement]);
    }

    public override void Load()
    {

        UpdateFocusedElementMethod = typeof(SilkyUIInputState).GetMethod("UpdateFocusedElement", BindingFlags.Instance | BindingFlags.NonPublic, [typeof(UIView)]);
        PressedElementsProperty = typeof(SilkyUIInputState).GetProperty("PressedElements", BindingFlags.Instance | BindingFlags.NonPublic);
        _inputStateField = typeof(SilkyUIManager).GetField("_inputState", BindingFlags.Instance | BindingFlags.NonPublic);

        base.Load();
    }
}