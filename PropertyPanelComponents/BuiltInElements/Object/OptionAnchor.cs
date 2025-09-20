using Microsoft.Xna.Framework;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using SilkyUIFramework;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Object;

public class OptionAnchor : OptionObject
{
    private AnchorObject anchorObj;

    protected override void FillOption()
    {
        base.FillOption();
        //PropertyPanel.Decorator =
        //    new CombinedDecorator(
        //        FitHeightDecorator.Instance,
        //        new AnchorDecorator(
        //            this));
        if (owner == null)
            PropertyPanel.Decorator = new AnchorDecorator(this);
        if (MetaData is ListValueHandler listHandler)
            anchorObj = new AnchorObject((IList<Anchor>)listHandler.List, listHandler.Index);
        else
            anchorObj = new AnchorObject(MetaData.VariableInfo, MetaData.Item);
        ShowStringValueInLabel = false;
    }

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegisterOptionToType(this, typeof(Anchor));
    }

    protected override IPropertyOptionFiller GetInternalPanelFiller(object data)
    {
        return base.GetInternalPanelFiller(anchorObj);
    }

    private class AnchorObject
    {
        private readonly PropertyFieldWrapper memberInfo;
        private readonly object item;
        private readonly IList<Anchor> array;
        private readonly int index;

        private Anchor current;

        [Range(0f, 100f)]
        public float Pixels
        {
            get => current.Pixels;
            set
            {
                current = new Anchor(value, current.Percent, current.Alignment);
                Update();
            }
        }

        public float Percent
        {
            get => current.Percent;
            set
            {
                current = new Anchor(current.Pixels, value, current.Alignment);
                Update();
            }
        }

        public float Alignment
        {
            get => current.Alignment;
            set
            {
                current = new Anchor(current.Pixels, current.Percent, value);
                Update();
            }
        }

        internal Anchor Anchor
        {
            get => current;
            set
            {
                current = value;
                Update();
            }
        }

        private void Update()
        {
            if (array == null)
                memberInfo.SetValue(item, current);
            else
                array[index] = current;
        }

        public AnchorObject(PropertyFieldWrapper memberInfo, object item)
        {
            this.item = item;
            this.memberInfo = memberInfo;
            current = (Anchor)memberInfo.GetValue(item);
        }

        public AnchorObject(IList<Anchor> array, int index)
        {
            current = array[index];
            this.array = array;
            this.index = index;
        }
    }

    private class AnchorDecorator(OptionAnchor optionAnchor) : IPropertyPanelDecorator
    {
        private UIElementGroup MaskPanel { get; set; }
        private UIView InnerView { get; set; }
        private OptionAnchor Option { get; set; } = optionAnchor;
        private Dimension OldWidth { get; set; }

        void IPropertyPanelDecorator.PostFillPanel(PropertyPanel panel)
        {
            panel.CrossAlignment = CrossAlignment.Center;
            var list = panel.OptionList;
            OldWidth = list.Width;
            const float height = 120f;
            list.SetWidth(-height - 8, 1);

            MaskPanel = new UIElementGroup
            {
                Margin = new Margin(8, 0, 8, 0),
                BackgroundColor = Color.Black * .4f
            };
            MaskPanel.SetSize(height, 40, 0, 0);
            MaskPanel.Join(panel);
            MaskPanel.OnUpdate += delegate
            {
                MaskPanel.SetHeight(Option._expandTimer.Schedule * 40, 0);
            };

            InnerView = new UIView
            {
                BackgroundColor = Color.Blue * .5f,
                Width = new Dimension(0, .2f),
                Height = new Dimension(0, 1),
            };
            InnerView.OnUpdate += delegate
            {
                InnerView.Left = (Anchor)Option.GetValue();
            };
            InnerView.Join(MaskPanel);
        }

        void IPropertyPanelDecorator.PreFillPanel(PropertyPanel panel)
        {
        }

        void IPropertyPanelDecorator.UnloadDecorate(PropertyPanel panel)
        {
            panel.CrossAlignment = CrossAlignment.Stretch;
            panel.OptionList.Width = OldWidth;
            MaskPanel?.Remove();
        }
    }
}