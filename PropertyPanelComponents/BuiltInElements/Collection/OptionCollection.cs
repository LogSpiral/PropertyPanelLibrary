using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using PropertyPanelLibrary.BasicElements;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Object;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.OptionDecorators;
using SilkyUIFramework.Extensions;
using System;
using System.Collections.Generic;
using SilkyUIFramework;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Collection;

public abstract partial class OptionCollection : OptionObject
{
    #region Attributes

    protected DefaultListValueAttribute DefaultListValueAttribute { get; set; }
    protected JsonDefaultListValueAttribute JsonDefaultListValueAttribute { get; set; }

    #endregion Attributes

    protected object Data { get; set; }
    protected virtual bool CanItemBeAdded => true;

    protected object CreateCollectionElementInstance(Type type)
    {
        object toAdd;

        if (DefaultListValueAttribute != null)
        {
            toAdd = DefaultListValueAttribute.Value;
        }
        else
        {
            toAdd = ConfigManager.AlternateCreateInstance(type);

            if (!type.IsValueType && type != typeof(string))
            {
                string json = JsonDefaultListValueAttribute?.Json ?? "{}";

                JsonConvert.PopulateObject(json, toAdd, ConfigManager.serializerSettings);
            }
        }

        return toAdd;
    }

    //protected void NetSyncManually()
    //{
    //    if (Main.netMode == NetmodeID.MultiplayerClient)
    //    {
    //        List<string> pathDirectly = [];
    //        if (path != null)
    //            pathDirectly.AddRange(path);
    //        if (List == null)
    //            pathDirectly.Add(VariableInfo.Name);
    //        ConfigOptionPacket.Send(Config, Data, pathDirectly);
    //    }
    //}

    protected abstract void PrepareTypes();

    protected abstract void AddItem();

    protected virtual void NullCollection()
    {
        Data = null;
        SetValue(Data);
        // NetSyncManually(); //TODO 联机同步相关处理
    }

    protected abstract void ClearCollection();

    private SUIPlus AddButton;

    protected override void CheckAttributes()
    {
        base.CheckAttributes();

        if (GetAttribute<DefaultListValueAttribute>() is { } defaultListValue)
            DefaultListValueAttribute = defaultListValue;
    }

    public override void CheckDesignagedAttributes(HashSet<Attribute> attributes)
    {
        base.CheckDesignagedAttributes(attributes);
        foreach (var attribute in attributes)
        {
            if (attribute is DefaultListValueAttribute defaultListValue)
                DefaultListValueAttribute ??= defaultListValue;
        }
    }

    protected override void FillOption()
    {
        PrepareTypes();
        Data = GetValue();
        base.FillOption();
        BuildAddButton();

        if (CanItemBeAdded)
            PropertyPanel.OptionDecorator =
                new CombinedOptionDecorator(
                    new RemoveElementDecorator(ClearCollection),
                    LabelOptionDecorator.NewLabelDecorator()
                    );
    }

    private void BuildAddButton()
    {
        AddButton = new SUIPlus
        {
            BackgroundColor = Color.Black * 0.4f,
            BorderRadius = new Vector4(4f),
            Width = new Dimension(25),
            Height = new Dimension(25),
            Margin = new Margin(4f, 0, 4, 0)
        };
        //ExpandButton.trianglePercentCoord[1] = new(0f, .5f);
        AddButton.LeftMouseClick += delegate
        {
            SoundEngine.PlaySound(SoundID.Tink);
            AddItem();
            pendingChanges = true;
            expanded = true;
        };
        if (CanItemBeAdded)
            AddButton.Join(ButtonContainer);
    }

    protected override void DeleteProcess()
    {
        if (NullAllowedAttribute != null)
            base.DeleteProcess();
        else
            ClearCollection();
    }

    protected override bool ShouldAppendDeleteButton() => true;
}