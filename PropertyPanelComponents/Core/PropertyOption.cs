using Microsoft.Xna.Framework;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.OptionDecorators;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using SilkyUIFramework;
using Terraria;

namespace PropertyPanelLibrary.PropertyPanelComponents.Core;

public partial class PropertyOption : UIElementGroup
{
    private IPropertyValueWriter Writer { get; set; }
    private IPropertyMouseHandler MouseHandler { get; set; }
    private IPropertyOptionInteractableHandler InteractableHandler { get; set; }
    private IPropertyOptionDecorator Decorator 
    {
        get => field ??= LabelOptionDecorator.NewLabelDecorator();
        set 
        {
            _pendingDecorateModified = true;
            field = value;
        }
    }
    private bool _pendingDecorateModified;
    public override void OnLeftMouseClick(UIMouseEvent evt)
    {
        if (evt.Source == this)
            MouseHandler?.LeftMouseClick(evt, this);
        base.OnLeftMouseClick(evt);
    }

    public override void OnRightMouseClick(UIMouseEvent evt)
    {
        if (evt.Source == this)
            MouseHandler?.RightMouseClick(evt, this);
        base.OnRightMouseClick(evt);
    }

    public override void OnMiddleMouseClick(UIMouseEvent evt)
    {
        if (evt.Source == this)
            MouseHandler?.MiddleMouseClick(evt, this);
        base.OnMiddleMouseClick(evt);
    }

    public void SetValue(object value, bool broadCast = true)
    {
        Writer?.WriteValue(this, value, broadCast);
        Main.NewText(value);
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);
        if (InteractableHandler != null) 
        {
            Interactable = InteractableHandler.CheckInteractable(this, out string message);
            InteractableMessage = message;
        }
        if (_pendingDecorateModified) 
        {
            Decorator?.UnloadDecorate(this);
            RemoveAllChildren();

            Decorator.PreFillOption(this);
            FillOption();
            Decorator.PostFillOption(this);
        }
    }
    public bool Interactable { get; private set; } = true;
    public string InteractableMessage { get; private set; } = "";

}