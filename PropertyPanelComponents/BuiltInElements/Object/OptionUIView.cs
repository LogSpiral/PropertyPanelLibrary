using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Decorators;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using SilkyUIFramework.Extensions;
using SilkyUIFramework;
using System.Linq;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;
using SilkyUIFramework.BasicElements;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Object;

public class OptionUIView : OptionObject
{
    protected override void FillOption()
    {
        base.FillOption();
        PropertyPanel.Decorator =
            new CombinedDecorator(
                FitHeightDecorator.Instance,
                new UIViewPanelDecorator(this));
    }



    protected override IPropertyOptionFiller GetInternalPanelFiller(object data)
    {
        return
            new DesignatedMemberFiller
            (
                (data,
                [nameof(Left),
                nameof(Top),
                nameof(Width),
                nameof(Height),
                nameof(Padding),
                nameof(Margin),
                nameof(BorderRadius)]
                ));
    }
    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegisterOptionToType(this, typeof(UIView));
    }
    private class UIViewPanelDecorator(OptionUIView optionUIView) : IPropertyPanelDecorator
    {
        OptionUIView Option { get; init; } = optionUIView;
        UIView View { get; set; }
        UIElementGroup MaskPanel { get; set; }
        Dimension OldWidth { get; set; }
        public void PostFillPanel(PropertyPanel panel)
        {
            var list = panel.OptionList;
            OldWidth = list.Width;
            const float height = 355.5f;
            list.SetWidth(-height - 8, 1);


            MaskPanel = new()
            {
                Margin = new(8, 0, 8, 0),
                BackgroundColor = Color.Black * .4f
            };
            MaskPanel.SetSize(height, 0, 0, 1);
            MaskPanel.Join(panel);
            if (Option.GetValue() is UIView view) 
            {
                view.Join(MaskPanel);
                View = view;
                View.BackgroundColor = Color.Blue * .5f;
            }
        }

        public void PreFillPanel(PropertyPanel panel)
        {
        }

        public void UnloadDecorate(PropertyPanel panel)
        {
            panel.OptionList.Width = OldWidth;
            MaskPanel?.Remove();
            View?.Remove();
        }
    }
}
