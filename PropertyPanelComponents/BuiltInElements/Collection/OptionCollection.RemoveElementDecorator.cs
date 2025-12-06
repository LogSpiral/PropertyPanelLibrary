using Microsoft.Xna.Framework;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using SilkyUIFramework;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Collection;

public partial class OptionCollection
{
    private class RemoveElementDecorator(Action onRemove = null) : IPropertyOptionDecorator
    {
        IPropertyOptionDecorator IPropertyOptionDecorator.Clone() => new RemoveElementDecorator(OnRemove);

        private Action OnRemove { get; init; } = onRemove;

        private SUICross RemoveButton { get; set; }

        void IPropertyOptionDecorator.PostFillOption(PropertyOption option)
        {
        }

        void IPropertyOptionDecorator.PreFillOption(PropertyOption option)
        {
            RemoveButton = new SUICross(SUIColor.Warn * .5f, SUIColor.Warn);
            RemoveButton.SetSize(25, 25);
            RemoveButton.Margin = new Margin(4f, 0, 4, 0);
            RemoveButton.BackgroundColor = Color.Black * .4f;
            RemoveButton.BorderRadius = new Vector4(4f);
            RemoveButton.Join(option);
            RemoveButton.LeftMouseClick += delegate
            {
                OnRemove?.Invoke();
            };
        }

        void IPropertyOptionDecorator.UnloadDecorate(PropertyOption option)
        {
            RemoveButton?.RemoveFromParent();
        }
    }
}