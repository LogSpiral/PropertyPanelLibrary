using Microsoft.Xna.Framework;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using SilkyUIFramework;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Object;

public class OptionVector2 : OptionObject
{
    private Vector2Object vecObj;

    protected override void FillOption()
    {
        base.FillOption();
        PropertyPanel.Decorator = new Vector2PanelDecorator(
                    this,
                    RangeAttribute,
                    IncrementAttribute);
        if (MetaData is ListValueHandler listHandler)
            vecObj = new((IList<Vector2>)listHandler.List, listHandler.Index);
        else
            vecObj = new(MetaData.VariableInfo, MetaData.Item);
        ShowStringValueInLabel = false;
    }

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegisterOptionToType(this, typeof(Vector2));
    }

    protected override IPropertyOptionFiller GetInternalPanelFiller(object data)
    {
        return base.GetInternalPanelFiller(vecObj);
    }

    private class Vector2Object
    {
        private readonly PropertyFieldWrapper memberInfo;
        private readonly object item;
        private readonly IList<Vector2> array;
        private readonly int index;

        private Vector2 current;

        public float X
        {
            get => current.X;
            set
            {
                current.X = value;
                Update();
            }
        }

        public float Y
        {
            get => current.Y;
            set
            {
                current.Y = value;
                Update();
            }
        }

        internal Vector2 Vec2
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

        public Vector2Object(PropertyFieldWrapper memberInfo, object item)
        {
            this.item = item;
            this.memberInfo = memberInfo;
            current = (Vector2)memberInfo.GetValue(item);
        }

        public Vector2Object(IList<Vector2> array, int index)
        {
            current = array[index];
            this.array = array;
            this.index = index;
        }
    }

    private class Vector2PanelDecorator(OptionVector2 optionVector2, RangeAttribute range, IncrementAttribute increment) : IPropertyPanelDecorator
    {
        private OptionVector2 Option { get; init; } = optionVector2;
        private RangeAttribute Range { get; init; } = range;
        private IncrementAttribute Increment { get; init; } = increment;
        private Vector2Panel VecPanel { get; set; }
        private Dimension OldWidth { get; set; }

        public void PostFillPanel(PropertyPanel panel)
        {
            var list = panel.OptionList;
            OldWidth = list.Width;
            const float height = 88;
            list.SetWidth(-height - 8, 1);

            float increment = 0;
            if (Increment != null)
                increment = (float)Increment.Increment;

            float min = 0;
            float max = 1;
            if (Range != null)
            {
                min = (float)Range.Min;
                max = (float)Range.Max;
            }
            VecPanel = new Vector2Panel((Vector2)Option.GetValue(), increment, min, max);
            VecPanel.OnUpdate += delegate
            {
                var vec = (Vector2)Option.GetValue();
                VecPanel.RealValue = vec;

                var factor = Option._expandTimer;
                VecPanel.YAxis.BackgroundColor = SUIColor.Warn * (.5f * factor);
                VecPanel.XAxis.BackgroundColor = SUIColor.Warn * (.5f * factor);
                VecPanel.PointPanel.BackgroundColor = SUIColor.Warn * (.75f * factor);
            };
            VecPanel.PointPanel.OnUpdate += delegate
            {
                Option.vecObj.Vec2 = VecPanel.RealValue;
            };
            VecPanel.Margin = new(8, 0, 8, 0);
            VecPanel.SetSize(height, 0, 0, 1);
            VecPanel.Join(panel);
        }

        public void PreFillPanel(PropertyPanel panel)
        {
        }

        public void UnloadDecorate(PropertyPanel panel)
        {
            panel.OptionList.Width = OldWidth;
            VecPanel?.Remove();
        }

        private class Vector2Panel : UIElementGroup
        {
            public float PercentX { get; set; }
            public float PercentY { get; set; }

            public UIView XAxis { get; set; }
            public UIView YAxis { get; set; }
            public UIView PointPanel { get; set; }

            public Vector2 RealValue
            {
                get
                {
                    Vector2 result = default;
                    float min = Min;
                    float max = Max;
                    float increment = Increment;

                    float value = min + (max - min) * PercentX;
                    if (increment != 0)
                        value = MathF.Round(value / increment) * increment;
                    value = Math.Clamp(value, min, max);
                    result.X = value;

                    value = min + (max - min) * PercentY;
                    if (increment != 0)
                        value = MathF.Round(value / increment) * increment;
                    value = Math.Clamp(value, min, max);
                    result.Y = value;
                    field = result;
                    return result;
                }
                set
                {
                    if (field == value) return;
                    float min = Min;
                    float max = Max;
                    float increment = Increment;
                    var coordValue = value.X;
                    if (increment != 0)
                        coordValue = MathF.Round(coordValue / increment) * increment;
                    PercentX = Utils.GetLerpValue(min, max, coordValue, true);

                    coordValue = value.Y;
                    if (increment != 0)
                        coordValue = MathF.Round(coordValue / increment) * increment;
                    PercentY = Utils.GetLerpValue(min, max, coordValue, true);
                }
            }

            public float Increment { get; init; }
            public float Min { get; init; }
            public float Max { get; init; }

            private bool _isDragging;
            private bool _lockX;
            private bool _lockY;

            public Vector2Panel(Vector2 initialValue, float increment, float min, float max)
            {
                LayoutType = LayoutType.Custom;
                var coordValue = initialValue.X;
                if (increment != 0)
                    coordValue = MathF.Round(coordValue / increment) * increment;
                PercentX = Utils.GetLerpValue(min, max, coordValue, true);

                coordValue = initialValue.Y;
                if (increment != 0)
                    coordValue = MathF.Round(coordValue / increment) * increment;
                PercentY = Utils.GetLerpValue(min, max, coordValue, true);

                Increment = increment;
                Min = min;
                Max = max;

                BackgroundColor = Color.Black * .3f;
                BorderRadius = new(4);

                float step = increment == 0 ? 0.2f : increment / (max - min);

                for (float k = 0; k <= 1; k += step)
                {
                    var XGrid = new UIView()
                    {
                        Width = new(2, 0),
                        Height = new(0, 1),
                        BackgroundColor = Color.Black * .2f,
                        Left = new(0, k - .5f, .5f)
                    };
                    XGrid.Join(this);
                    var YGrid = new UIView()
                    {
                        Height = new(2, 0),
                        Width = new(0, 1),
                        BackgroundColor = Color.Black * .2f,
                        Top = new(0, k - .5f, .5f)
                    };
                    YGrid.Join(this);
                }

                XAxis = new UIView()
                {
                    Width = new(4, 0),
                    Height = new(0, 1),
                    BackgroundColor = SUIColor.Warn * .5f
                };
                XAxis.OnUpdate += delegate
                {
                    XAxis.SetLeft(0, PercentX - .5f, .5f);
                };
                XAxis.LeftMouseDown += delegate
                {
                    _lockY = true;
                    _isDragging = true;
                };
                XAxis.LeftMouseUp += delegate
                {
                    _lockY = false;
                    _isDragging = false;
                };
                XAxis.Join(this);
                YAxis = new UIView()
                {
                    Height = new(4, 0),
                    Width = new(0, 1),
                    BackgroundColor = SUIColor.Warn * .5f
                };
                YAxis.OnUpdate += delegate
                {
                    YAxis.SetTop(0, PercentY - .5f, .5f);
                };
                YAxis.LeftMouseDown += delegate
                {
                    _lockX = true;
                    _isDragging = true;
                };
                YAxis.LeftMouseUp += delegate
                {
                    _lockX = false;
                    _isDragging = false;
                };
                YAxis.Join(this);
                PointPanel = new UIView()
                {
                    Height = new(8, 0),
                    Width = new(8, 0),
                    BackgroundColor = SUIColor.Warn * .75f,
                    BorderRadius = new(4f)
                };
                PointPanel.OnUpdate += delegate
                {
                    PointPanel.SetLeft(0, PercentX - .5f, .5f);
                    PointPanel.SetTop(0, PercentY - .5f, .5f);
                };
                PointPanel.LeftMouseDown += delegate
                {
                    _isDragging = true;
                };
                PointPanel.LeftMouseUp += delegate
                {
                    _isDragging = false;
                };
                PointPanel.Join(this);
                LeftMouseDown += (sender, evt) =>
                {
                    _isDragging = true;
                    var bounds = Bounds;
                    if (!_lockX)
                        PercentX = MathHelper.Clamp((Main.MouseScreen.X - bounds.X) / bounds.Width, 0, 1);
                    if (!_lockY)
                        PercentY = MathHelper.Clamp((Main.MouseScreen.Y - bounds.Y) / bounds.Height, 0, 1);
                };
            }

            protected override void UpdateStatus(GameTime gameTime)
            {
                if (_isDragging)
                {
                    var bounds = Bounds;
                    if (!_lockX)
                        PercentX = MathHelper.Clamp((Main.MouseScreen.X - bounds.X) / bounds.Width, 0, 1);
                    if (!_lockY)
                        PercentY = MathHelper.Clamp((Main.MouseScreen.Y - bounds.Y) / bounds.Height, 0, 1);
                }
                base.UpdateStatus(gameTime);
            }
        }
    }
}