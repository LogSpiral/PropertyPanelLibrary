using Microsoft.Xna.Framework;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using SilkyUIFramework;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Object;

internal class OptionVector4 : OptionObject
{
    private Vector4Object vecObj;

    protected override void FillOption()
    {
        base.FillOption();
        PropertyPanel.Decorator = new Vector4PanelDecorator(
                    this,
                    RangeAttribute,
                    IncrementAttribute);
        if (MetaData is ListValueHandler listHandler)
            vecObj = new((IList<Vector4>)listHandler.List, listHandler.Index);
        else
            vecObj = new(MetaData.VariableInfo, MetaData.Item);
        ShowStringValueInLabel = false;
    }

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegisterOptionToType(this, typeof(Vector4));
    }

    protected override IPropertyOptionFiller GetInternalPanelFiller(object data)
    {
        return base.GetInternalPanelFiller(vecObj) as ObjectMetaDataFiller;
    }

    private class Vector4Object
    {
        private readonly PropertyFieldWrapper memberInfo;
        private readonly object item;
        private readonly IList<Vector4> array;
        private readonly int index;

        private Vector4 current;

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

        public float Z
        {
            get => current.Z;
            set
            {
                current.Z = value;
                Update();
            }
        }

        public float W
        {
            get => current.W;
            set
            {
                current.W = value;
                Update();
            }
        }

        internal Vector4 Vec4
        {
            get => current;
            set
            {
                current = value;
                Update();
            }
        }

        internal Vector2 XY
        {
            get => current.XY();
            set
            {
                current = current with { X = value.X, Y = value.Y };
                Update();
            }
        }

        internal Vector2 ZW
        {
            get => current.ZW();
            set
            {
                current = current with { Z = value.X, W = value.Y };
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

        public Vector4Object(PropertyFieldWrapper memberInfo, object item)
        {
            this.item = item;
            this.memberInfo = memberInfo;
            current = (Vector4)memberInfo.GetValue(item);
        }

        public Vector4Object(IList<Vector4> array, int index)
        {
            current = array[index];
            this.array = array;
            this.index = index;
        }
    }

    private class Vector4PanelDecorator(OptionVector4 optionVector4, RangeAttribute range, IncrementAttribute increment) : IPropertyPanelDecorator
    {
        private OptionVector4 Option { get; init; } = optionVector4;
        private RangeAttribute Range { get; init; } = range;
        private IncrementAttribute Increment { get; init; } = increment;
        private Vector4Panel VecPanelXY { get; set; }
        private Vector4Panel VecPanelZW { get; set; }

        private UIElementGroup PanelMask { get; set; }
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
            PanelMask = new();
            PanelMask.SetSize(height, 0, 0, 1);
            PanelMask.FlexWrap = true;
            PanelMask.FlexDirection = FlexDirection.Column;
            PanelMask.MainAlignment = MainAlignment.SpaceBetween;
            PanelMask.Join(panel);

            VecPanelXY = new Vector4Panel(((Vector4)Option.GetValue()).XY(), increment, min, max);
            VecPanelXY.OnUpdate += delegate
            {
                var vec = (Vector4)Option.GetValue();
                VecPanelXY.RealValue = vec.XY();

                var factor = Option._expandTimer;
                VecPanelXY.YAxis.BackgroundColor = SUIColor.Warn * (.5f * factor);
                VecPanelXY.XAxis.BackgroundColor = SUIColor.Warn * (.5f * factor);
                VecPanelXY.PointPanel.BackgroundColor = SUIColor.Warn * (.75f * factor);
            };
            VecPanelXY.PointPanel.OnUpdate += delegate
            {
                Option.vecObj.XY = VecPanelXY.RealValue;
            };
            VecPanelXY.Margin = new(8, 0, 8, 0);
            VecPanelXY.SetSize(height, -4, 0, .5f);
            VecPanelXY.Join(PanelMask);

            VecPanelZW = new Vector4Panel(((Vector4)Option.GetValue()).ZW(), increment, min, max);
            VecPanelZW.OnUpdate += delegate
            {
                var vec = (Vector4)Option.GetValue();
                VecPanelZW.RealValue = vec.ZW();

                var factor = Option._expandTimer;
                VecPanelZW.YAxis.BackgroundColor = SUIColor.Warn * (.5f * factor);
                VecPanelZW.XAxis.BackgroundColor = SUIColor.Warn * (.5f * factor);
                VecPanelZW.PointPanel.BackgroundColor = SUIColor.Warn * (.75f * factor);
            };
            VecPanelZW.PointPanel.OnUpdate += delegate
            {
                Option.vecObj.ZW = VecPanelZW.RealValue;
            };
            VecPanelZW.Margin = new(8, 0, 8, 0);
            VecPanelZW.SetSize(height, -4, 0, .5f);
            VecPanelZW.Join(PanelMask);
        }

        public void PreFillPanel(PropertyPanel panel)
        {
        }

        public void UnloadDecorate(PropertyPanel panel)
        {
            panel.OptionList.Width = OldWidth;
            PanelMask?.Remove();
        }

        private class Vector4Panel : UIElementGroup
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

            public Vector4Panel(Vector2 initialValue, float increment, float min, float max)
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