using Microsoft.Xna.Framework;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using SilkyUIFramework;
using SilkyUIFramework.Layout;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Object;

public class OptionDimension : OptionObject
{
    private DimensionObject DimensionObj;

    protected override void FillOption()
    {
        base.FillOption();
        if (owner == null)
            PropertyPanel.Decorator = new DimensionDecorator(this);
        if (MetaData is ListValueHandler listHandler)
            DimensionObj = new DimensionObject((IList<Dimension>)listHandler.List, listHandler.Index);
        else
            DimensionObj = new DimensionObject(MetaData.VariableInfo, MetaData.Item);
        ShowStringValueInLabel = false;
    }

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegisterOptionToType(this, typeof(Dimension));
    }

    protected override IPropertyOptionFiller GetInternalPanelFiller(object data)
    {
        return base.GetInternalPanelFiller(DimensionObj);
    }

    private class DimensionObject
    {
        private readonly PropertyFieldWrapper memberInfo;
        private readonly object item;
        private readonly IList<Dimension> array;
        private readonly int index;

        private Dimension current;

        [Range(0f, 100f)]
        public float Pixels
        {
            get => current.Pixels;
            set
            {
                current = new Dimension(value, current.Percent);
                Update();
            }
        }

        public float Percent
        {
            get => current.Percent;
            set
            {
                current = new Dimension(current.Pixels, value);
                Update();
            }
        }

        internal Dimension Dimension
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

        public DimensionObject(PropertyFieldWrapper memberInfo, object item)
        {
            this.item = item;
            this.memberInfo = memberInfo;
            current = (Dimension)memberInfo.GetValue(item);
        }

        public DimensionObject(IList<Dimension> array, int index)
        {
            current = array[index];
            this.array = array;
            this.index = index;
        }
    }

    private class DimensionDecorator(OptionDimension optionDimension) : IPropertyPanelDecorator
    {
        private UIElementGroup MaskPanel { get; set; }
        private UIView InnerView { get; set; }
        private OptionDimension Option { get; set; } = optionDimension;
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
                InnerView.Width = (Dimension)Option.GetValue();
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
            MaskPanel?.RemoveFromParent();
        }
    }
}