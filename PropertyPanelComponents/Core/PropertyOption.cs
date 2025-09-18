using Microsoft.Xna.Framework;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.InteractableHandlers;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.MouseHandlers;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.OptionDecorators;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.Writers;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using SilkyUIFramework;
using SilkyUIFramework.Elements;

namespace PropertyPanelLibrary.PropertyPanelComponents.Core;

public partial class PropertyOption : UIElementGroup
{
    public IPropertyValueWriter Writer
    {
        protected get => field ??= DefaultWriter.Instance;
        set;
    }

    public IPropertyMouseHandler MouseHandler
    {
        protected get => field ??= NoneMouseHandler.Instance;
        set;
    }

    public IPropertyOptionInteractableHandler InteractableHandler
    {
        protected get => field ??= NoneInteractableHandler.Instance;
        set;
    }

    public IPropertyOptionDecorator Decorator
    {
        protected get => field ??= LabelOptionDecorator.NewLabelDecorator();
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
        // Main.NewText(value == null ? "null" : value.ToString());
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
            _pendingDecorateModified = false;
            Decorator.UnloadDecorate(this);
            RemoveAllChildren();

            Decorator.PreFillOption(this);
            FillOption();
            Decorator.PostFillOption(this);
        }
    }

    public bool Interactable { get; private set; } = true;
    public string InteractableMessage { get; private set; } = "";
}