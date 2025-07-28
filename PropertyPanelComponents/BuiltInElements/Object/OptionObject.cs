using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Decorators;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;
using SilkyUIFramework;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Object;

public class OptionObject : PropertyOption
{
    private PropertyPanel PropertyPanel { get; set; }
    private UITextView InitiateButton { get; set; }
    protected JsonDefaultValueAttribute JsonDefaultValueAttribute;

    protected override void FillOption()
    {
        FitHeight = true;
        LayoutType = LayoutType.Flexbox;
        FlexDirection = FlexDirection.Column;

        //SUIDividingLine.Horizontal(Color.Black * .75f).Join(this);

        PropertyPanel = new PropertyPanel();
        PropertyPanel.SetWidth(0, 1);
        PropertyPanel.Join(this);
        PropertyPanel.Decorator = FitHeightDecorator.Instance;

        var target = GetValue();
        if (target == null)
        {
            InitiateButton = new UITextView();
            InitiateButton.Text = "实例化";
            InitiateButton.SetLeft(0, 0, 1);
            InitiateButton.SetTop(-60, 0, 1f);
            InitiateButton.SetWidth(80, 0);
            InitiateButton.BackgroundColor = Color.Black * .25f;
            InitiateButton.Border = 2f;
            InitiateButton.BorderRadius = new(8);
            InitiateButton.BorderColor = SUIColor.Border;
            InitiateButton.LeftMouseClick += delegate
                {
                    SoundEngine.PlaySound(SoundID.Tink);
                    object data = Activator.CreateInstance(VariableType, true);
                    string json = JsonDefaultValueAttribute?.Json ?? "{}";
                    JsonConvert.PopulateObject(json, data, ConfigManager.serializerSettings);
                    SetValue(data);
                    PropertyPanel.Filler = new ObjectMetaDataFiller(data);
                    InitiateButton.Remove();
                };
            InitiateButton.Join(this);
        }
        else
        {
            PropertyPanel.Filler = new ObjectMetaDataFiller(target);
        }
    }

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegistreOptionToTypeComplex(this, type =>
        {
            if (type.IsArray) return false;
            if (type.IsGenericType)
            {
                var definition = type.GetGenericTypeDefinition();
                if (definition == typeof(List<>)) return false;
                if (definition == typeof(Dictionary<,>)) return false;
                if (definition == typeof(HashSet<>)) return false;
            }
            return true;
        });
    }
}