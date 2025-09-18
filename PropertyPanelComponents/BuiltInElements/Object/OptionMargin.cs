using Microsoft.Xna.Framework;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using SilkyUIFramework;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Object;

public class OptionMargin : OptionObject
{
    private MarginObject MarginObj;

    protected override void FillOption()
    {
        base.FillOption();
        if (owner == null)
            PropertyPanel.Decorator = new MarginDecorator(this);
        if (MetaData is ListValueHandler listHandler)
            MarginObj = new((IList<Margin>)listHandler.List, listHandler.Index);
        else
            MarginObj = new(MetaData.VariableInfo, MetaData.Item);
        ShowStringValueInLabel = false;
    }

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegisterOptionToType(this, typeof(Margin));
    }

    protected override IPropertyOptionFiller GetInternalPanelFiller(object data)
    {
        return base.GetInternalPanelFiller(MarginObj);
    }

    private class MarginObject
    {
        private readonly PropertyFieldWrapper memberInfo;
        private readonly object item;
        private readonly IList<Margin> array;
        private readonly int index;

        private Margin current;

        [Range(0f, 20f)]
        public float Left
        {
            get => current.Left;
            set
            {
                current = new(value, current.Top, current.Right, current.Bottom);
                Update();
            }
        }

        [Range(0f, 20f)]
        public float Top
        {
            get => current.Top;
            set
            {
                current = new(current.Left, value, current.Right, current.Bottom);
                Update();
            }
        }

        [Range(0f, 20f)]
        public float Right
        {
            get => current.Right;
            set
            {
                current = new(current.Left, current.Top, value, current.Bottom);
                Update();
            }
        }

        [Range(0f, 20f)]
        public float Bottom
        {
            get => current.Bottom;
            set
            {
                current = new(current.Left, current.Top, current.Right, value);
                Update();
            }
        }

        internal Margin Margin
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

        public MarginObject(PropertyFieldWrapper memberInfo, object item)
        {
            this.item = item;
            this.memberInfo = memberInfo;
            current = (Margin)memberInfo.GetValue(item);
        }

        public MarginObject(IList<Margin> array, int index)
        {
            current = array[index];
            this.array = array;
            this.index = index;
        }
    }

    private class MarginDecorator(OptionMargin optionMargin) : IPropertyPanelDecorator
    {
        private UIElementGroup MaskPanelMargin { get; set; }
        private UIElementGroup MaskPanelPadding { get; set; }
        private UIElementGroup MaskContainer { get; set; }
        private UIView InnerViewMargin { get; set; }
        private UIView InnerViewPadding { get; set; }
        private OptionMargin Option { get; set; } = optionMargin;
        private Dimension OldWidth { get; set; }

        void IPropertyPanelDecorator.PostFillPanel(PropertyPanel panel)
        {
            var list = panel.OptionList;
            OldWidth = list.Width;
            const float height = 120f;
            list.SetWidth(-height - 8, 1);

            MaskContainer = new();
            MaskContainer.SetSize(height, 0, 0, 1);
            MaskContainer.FlexWrap = true;
            MaskContainer.FlexDirection = FlexDirection.Column;
            MaskContainer.MainAlignment = MainAlignment.SpaceBetween;
            MaskContainer.Join(panel);

            #region Margin

            MaskPanelMargin = new()
            {
                Margin = new(8, 0, 8, 0),
                BackgroundColor = Color.Black * .4f
            };
            MaskPanelMargin.SetSize(height, 88, 0, 0);
            MaskPanelMargin.Join(MaskContainer);
            MaskPanelMargin.OnUpdate += delegate
            {
                MaskPanelMargin.SetHeight(Option._expandTimer.Schedule * 88, 0);
            };
            MaskPanelMargin.LayoutType = LayoutType.Custom;

            InnerViewMargin = new()
            {
                BackgroundColor = Color.Blue * .5f,
                Width = new(0, .2f),
                Height = new(0, 0.4f),
                Left = new(0, 0, 0.5f),
                Top = new(0, 0, 0.5f)
            };
            InnerViewMargin.OnUpdate += delegate
            {
                InnerViewMargin.Margin = (Margin)Option.GetValue();
            };

            UIView marginBounds = new()
            {
                BackgroundColor = Color.White * .1f,
                Width = new(0, .2f),
                Height = new(0, 0.4f),
                Left = new(0, 0, 0.5f),
                Top = new(0, 0, 0.5f)
            };
            marginBounds.Join(MaskPanelMargin);
            marginBounds.OnUpdate += delegate
            {
                var margin = InnerViewMargin.Margin;
                float extraWidth = margin.Right + margin.Left;
                float extraHeight = margin.Bottom + margin.Top;
                marginBounds.SetWidth(extraWidth);
                marginBounds.SetHeight(extraHeight);
            };

            UIView marginBoundsAni = new()
            {
                BackgroundColor = Color.White * .1f,
                Width = new(0, .2f),
                Height = new(0, 0.4f),
                Left = new(0, 0, 0.5f),
                Top = new(0, 0, 0.5f)
            };
            marginBoundsAni.Join(MaskPanelMargin);
            marginBoundsAni.OnUpdate += delegate
            {
                var margin = InnerViewMargin.Margin;
                float extraWidth = margin.Right + margin.Left;
                float extraHeight = margin.Bottom + margin.Top;

                float factor = Main.GlobalTimeWrappedHourly % 1;
                extraWidth *= factor;
                extraHeight *= factor;
                factor = 1 - factor;
                marginBoundsAni.BackgroundColor = Color.White * (.1f * factor);

                marginBoundsAni.SetWidth(extraWidth);
                marginBoundsAni.SetHeight(extraHeight);
                marginBoundsAni.SetLeft((margin.Left - margin.Right) * .5f * factor, 0f);
                marginBoundsAni.SetTop((margin.Top - margin.Bottom) * .5f * factor, 0f);
            };

            InnerViewMargin.Join(MaskPanelMargin);

            #endregion Margin

            #region Padding

            MaskPanelPadding = new()
            {
                Margin = new(8, 0, 8, 0),
                BackgroundColor = Color.Black * .7f
            };
            MaskPanelPadding.SetSize(height, 88, 0, 0);
            MaskPanelPadding.Join(MaskContainer);
            MaskPanelPadding.OnUpdate += delegate
            {
                MaskPanelPadding.SetHeight(Option._expandTimer.Schedule * 88, 0);
                // MaskPanelPadding.Padding = (Margin)Option.GetValue();
            };
            MaskPanelPadding.LayoutType = LayoutType.Custom;

            InnerViewPadding = new()
            {
                BackgroundColor = Color.Blue * .5f,
                Width = new(0, .2f),
                Height = new(0, 0.4f),
                Left = new(0, 0, 0.5f),
                Top = new(0, 0, 0.5f)
            };

            UIView paddingBounds = new()
            {
                BackgroundColor = Color.White * .1f,
                Width = new(0, 1),
                Height = new(0, 1),
                Left = new(0, 0, 0.5f),
                Top = new(0, 0, 0.5f)
            };
            paddingBounds.Join(MaskPanelPadding);
            paddingBounds.OnUpdate += delegate
            {
                var padding = (Margin)Option.GetValue();
                float extraWidth = padding.Right + padding.Left;
                float extraHeight = padding.Bottom + padding.Top;
                paddingBounds.SetWidth(-extraWidth);
                paddingBounds.SetHeight(-extraHeight);

                paddingBounds.SetLeft((padding.Left - padding.Right) * .5f);
                paddingBounds.SetTop((padding.Top - padding.Bottom) * .5f);
            };

            UIView paddingBoundsAni = new()
            {
                BackgroundColor = Color.White * .1f,
                Width = new(0, 1),
                Height = new(0, 1),
                Left = new(0, 0, 0.5f),
                Top = new(0, 0, 0.5f)
            };
            paddingBoundsAni.Join(MaskPanelPadding);
            paddingBoundsAni.OnUpdate += delegate
            {
                var padding = (Margin)Option.GetValue();
                float extraWidth = padding.Right + padding.Left;
                float extraHeight = padding.Bottom + padding.Top;

                float factor = Main.GlobalTimeWrappedHourly % 1;
                extraWidth *= factor;
                extraHeight *= factor;
                // paddingBoundsAni.BackgroundColor = Color.White * (.1f * factor);
                //factor = 1 - factor;
                paddingBoundsAni.SetWidth(-extraWidth);
                paddingBoundsAni.SetHeight(-extraHeight);
                paddingBoundsAni.SetLeft((padding.Left - padding.Right) * factor * .5f, 0f);
                paddingBoundsAni.SetTop((padding.Top - padding.Bottom) * factor * .5f, 0f);
            };

            InnerViewPadding.Join(MaskPanelPadding);

            #endregion Padding
        }

        void IPropertyPanelDecorator.PreFillPanel(PropertyPanel panel)
        {
        }

        void IPropertyPanelDecorator.UnloadDecorate(PropertyPanel panel)
        {
            panel.OptionList.Width = OldWidth;
            MaskContainer?.Remove();
        }
    }
}