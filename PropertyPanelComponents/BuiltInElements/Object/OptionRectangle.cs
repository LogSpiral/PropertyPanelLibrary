using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Decorators;
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

public class OptionRectangle : OptionObject
{
    RectangleObject rectangleObj;
    protected override void FillOption()
    {
        base.FillOption();
        PropertyPanel.Decorator = new RectanglePanelDecorator(
                    this,
                    RangeAttribute,
                    IncrementAttribute);
        if (MetaData is ListValueHandler listHandler)
            rectangleObj = new((IList<Rectangle>)listHandler.List, listHandler.Index);
        else
            rectangleObj = new(MetaData.VariableInfo, MetaData.Item);
        ShowStringValueInLabel = false;
    }

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegisterOptionToType(this, typeof(Rectangle));
    }
    protected override IPropertyOptionFiller GetInternalPanelFiller(object data)
    {
        return base.GetInternalPanelFiller(rectangleObj);
    }
    private class RectangleObject
    {
        private readonly PropertyFieldWrapper memberInfo;
        private readonly object item;
        private readonly IList<Rectangle> array;
        private readonly int index;

        private Rectangle current;

        public int X
        {
            get => current.X;
            set
            {
                current.X = value;
                Update();
            }
        }

        public int Y
        {
            get => current.Y;
            set
            {
                current.Y = value;
                Update();
            }
        }

        public int Width
        {
            get => current.Width;
            set
            {
                current.Width = value;
                Update();
            }
        }

        public int Height
        {
            get => current.Height;
            set
            {
                current.Height = value;
                Update();
            }
        }

        internal Rectangle Rect
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

        public RectangleObject(PropertyFieldWrapper memberInfo, object item)
        {
            this.item = item;
            this.memberInfo = memberInfo;
            current = (Rectangle)memberInfo.GetValue(item);
        }

        public RectangleObject(IList<Rectangle> array, int index)
        {
            current = array[index];
            this.array = array;
            this.index = index;
        }
    }
    private class RectanglePanelDecorator(OptionRectangle optionRectangle, RangeAttribute range, IncrementAttribute increment) : IPropertyPanelDecorator
    {
        OptionRectangle Option { get; init; } = optionRectangle;
        RangeAttribute Range { get; init; } = range;
        IncrementAttribute Increment { get; init; } = increment;
        RectanglePanel RectPanel { get; set; }
        Dimension OldWidth { get; set; }
        public void PostFillPanel(PropertyPanel panel)
        {
            var list = panel.OptionList;
            OldWidth = list.Width;
            const float height = 184;
            list.SetWidth(-height - 8, 1);

            float increment = 0;
            if (Increment != null)
                increment = (float)Increment.Increment;

            float min = 0;
            float max = 100;
            if (Range != null)
            {
                min = (float)Range.Min;
                max = (float)Range.Max;
            }
            RectPanel = new RectanglePanel((Rectangle)Option.GetValue(), increment, min, max);
            RectPanel.OnUpdate += delegate
            {
                var vec = (Rectangle)Option.GetValue();
                RectPanel.RealValue = vec;

                var factor = Option._expandTimer;
                RectPanel.TopAxis.BackgroundColor = SUIColor.Warn * (.5f * factor);
                RectPanel.LeftAxis.BackgroundColor = SUIColor.Warn * (.5f * factor);
                RectPanel.StartPanel.BackgroundColor = SUIColor.Warn * (.75f * factor);
                RectPanel.BottomAxis.BackgroundColor = SUIColor.Highlight * (.5f * factor);
                RectPanel.RightAxis.BackgroundColor = SUIColor.Highlight* (.5f * factor);
                RectPanel.EndPanel.BackgroundColor = SUIColor.Highlight * (.75f * factor);

            };
            RectPanel.EndPanel.OnUpdate += delegate
            {
                Option.rectangleObj.Rect = RectPanel.RealValue;
            };
            RectPanel.Margin = new(8, 0, 8, 0);
            RectPanel.SetSize(height, 0, 0, 1);
            RectPanel.Join(panel);
        }

        public void PreFillPanel(PropertyPanel panel)
        {
        }

        public void UnloadDecorate(PropertyPanel panel)
        {
            panel.OptionList.Width = OldWidth;
            RectPanel?.Remove();
        }


        private class RectanglePanel : UIElementGroup
        {
            public float PercentLeft { get; set; }
            public float PercentTop { get; set; }
            public float PercentRight { get; set; }
            public float PercentBottom { get; set; }
            public UIView LeftAxis { get; set; }
            public UIView TopAxis { get; set; }

            public UIView RightAxis { get; set; }
            public UIView BottomAxis { get; set; }

            public UIView StartPanel { get; set; }
            public UIView EndPanel { get; set; }

            public Rectangle RealValue
            {
                get
                {
                    Rectangle result = default;
                    float min = Min;
                    float max = Max;
                    float increment = Increment;

                    float value = min + (max - min) * PercentLeft;
                    if (increment != 0)
                        value = MathF.Round(value / increment) * increment;
                    value = Math.Clamp(value, min, max);
                    result.X = (int)value;

                    value = min + (max - min) * PercentTop;
                    if (increment != 0)
                        value = MathF.Round(value / increment) * increment;
                    value = Math.Clamp(value, min, max);
                    result.Y = (int)value;

                    value = min + (max - min) * PercentRight;
                    if (increment != 0)
                        value = MathF.Round(value / increment) * increment;
                    value = Math.Clamp(value, min, max);
                    result.Width = (int)value - result.X;

                    value = min + (max - min) * PercentBottom;
                    if (increment != 0)
                        value = MathF.Round(value / increment) * increment;
                    value = Math.Clamp(value, min, max);
                    result.Height = (int)value - result.Y;

                    field = result;
                    return result;
                }
                set
                {
                    if (field == value) return;
                    float min = Min;
                    float max = Max;
                    float increment = Increment;
                    SyncValue(value, min, max, increment);
                }
            }
            void SyncValue(Rectangle value, float min, float max, float increment)
            {
                bool shouldRound = increment != 0;
                float coordValue = value.X;
                if (shouldRound)
                    coordValue = MathF.Round(coordValue / increment) * increment;
                PercentLeft = Utils.GetLerpValue(min, max, coordValue, true);

                coordValue = value.Y;
                if (shouldRound)
                    coordValue = MathF.Round(coordValue / increment) * increment;
                PercentTop = Utils.GetLerpValue(min, max, coordValue, true);

                coordValue = value.Width;
                if (shouldRound)
                    coordValue = MathF.Round(coordValue / increment) * increment;
                PercentRight = Utils.GetLerpValue(min, max, coordValue, true) + PercentLeft;

                coordValue = value.Height;
                if (shouldRound)
                    coordValue = MathF.Round(coordValue / increment) * increment;
                PercentBottom = Utils.GetLerpValue(min, max, coordValue, true) + PercentTop;
            }
            public float Increment { get; init; }
            public float Min { get; init; }
            public float Max { get; init; }

            bool _isDraggingStartPoint;
            bool _isDraggingEndPoint;
            bool _lockX;
            bool _lockY;
            public RectanglePanel(Rectangle initialValue, float increment, float min, float max)
            {
                LayoutType = LayoutType.Custom;
                SyncValue(initialValue, min, max, increment);

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
                #region LeftAxis
                LeftAxis = new UIView()
                {
                    Width = new(4, 0),
                    Height = new(0, 1),
                    BackgroundColor = SUIColor.Warn * .5f
                };
                LeftAxis.OnUpdate += delegate
                {
                    LeftAxis.SetLeft(0, PercentLeft - .5f, .5f);
                };
                LeftAxis.LeftMouseDown += delegate
                {
                    _lockY = true;
                    _isDraggingStartPoint = true;
                };
                LeftAxis.LeftMouseUp += delegate
                {
                    _lockY = false;
                    _isDraggingStartPoint = false;
                };
                LeftAxis.Join(this);
                #endregion

                #region RightAxis
                RightAxis = new UIView()
                {
                    Width = new(4, 0),
                    Height = new(0, 1),
                    BackgroundColor = SUIColor.Highlight * .5f
                };
                RightAxis.OnUpdate += delegate
                {
                    RightAxis.SetLeft(0, PercentRight - .5f, .5f);
                };
                RightAxis.RightMouseDown += delegate
                {
                    _lockY = true;
                    _isDraggingEndPoint = true;
                };
                RightAxis.RightMouseUp += delegate
                {
                    _lockY = false;
                    _isDraggingEndPoint = false;
                };
                RightAxis.Join(this);
                #endregion

                #region TopAxis
                TopAxis = new UIView()
                {
                    Height = new(4, 0),
                    Width = new(0, 1),
                    BackgroundColor = SUIColor.Warn * .5f
                };
                TopAxis.OnUpdate += delegate
                {
                    TopAxis.SetTop(0, PercentTop - .5f, .5f);
                };
                TopAxis.LeftMouseDown += delegate
                {
                    _lockX = true;
                    _isDraggingStartPoint = true;
                };
                TopAxis.LeftMouseUp += delegate
                {
                    _lockX = false;
                    _isDraggingStartPoint = false;
                };
                TopAxis.Join(this);
                #endregion

                #region BottomAxis
                BottomAxis = new UIView()
                {
                    Height = new(4, 0),
                    Width = new(0, 1),
                    BackgroundColor = SUIColor.Warn * .5f
                };
                BottomAxis.OnUpdate += delegate
                {
                    BottomAxis.SetTop(0, PercentBottom - .5f, .5f);
                };
                BottomAxis.LeftMouseDown += delegate
                {
                    _lockX = true;
                    _isDraggingEndPoint = true;
                };
                BottomAxis.LeftMouseUp += delegate
                {
                    _lockX = false;
                    _isDraggingEndPoint = false;
                };
                BottomAxis.Join(this);
                #endregion

                #region StartPanel
                StartPanel = new UIView()
                {
                    Height = new(8, 0),
                    Width = new(8, 0),
                    BackgroundColor = SUIColor.Warn * .75f,
                    BorderRadius = new(4f)
                };
                StartPanel.OnUpdate += delegate
                {
                    StartPanel.SetLeft(0, PercentLeft - .5f, .5f);
                    StartPanel.SetTop(0, PercentTop - .5f, .5f);
                };
                StartPanel.LeftMouseDown += delegate
                {
                    _isDraggingStartPoint = true;
                };
                StartPanel.LeftMouseUp += delegate
                {
                    _isDraggingStartPoint = false;
                };
                StartPanel.Join(this);
                #endregion

                #region EndPanel
                EndPanel = new UIView()
                {
                    Height = new(8, 0),
                    Width = new(8, 0),
                    BackgroundColor = SUIColor.Warn * .75f,
                    BorderRadius = new(4f)
                };
                EndPanel.OnUpdate += delegate
                {
                    EndPanel.SetLeft(0, PercentRight - .5f, .5f);
                    EndPanel.SetTop(0, PercentBottom - .5f, .5f);
                };
                EndPanel.LeftMouseDown += delegate
                {
                    _isDraggingEndPoint = true;
                };
                EndPanel.LeftMouseUp += delegate
                {
                    _isDraggingEndPoint = false;
                };
                EndPanel.Join(this);
                #endregion
                //LeftMouseDown += (sender, evt) =>
                //{
                //    _isDragging = true;
                //    var bounds = Bounds;
                //    if (!_lockX)
                //        PercentLeft = MathHelper.Clamp((Main.MouseScreen.X - bounds.X) / bounds.Width, 0, 1);
                //    if (!_lockY)
                //        PercentTop = MathHelper.Clamp((Main.MouseScreen.Y - bounds.Y) / bounds.Height, 0, 1);
                //};

                UIView viewPanel = new();
                viewPanel.BackgroundColor = Color.Cyan * .25f;
                viewPanel.OnUpdate += delegate
                {
                    viewPanel.SetLeft(0, PercentLeft);
                    viewPanel.SetWidth(0, PercentRight - PercentLeft);
                    viewPanel.SetTop(0, PercentTop);
                    viewPanel.SetHeight(0, PercentBottom - PercentTop);
                };
                viewPanel.IgnoreMouseInteraction = true;
                viewPanel.Join(this);
            }

            protected override void UpdateStatus(GameTime gameTime)
            {
                if (_isDraggingStartPoint)
                {
                    var bounds = Bounds;
                    if (!_lockX)
                        PercentLeft = MathHelper.Clamp((Main.MouseScreen.X - bounds.X) / bounds.Width, 0, 1);
                    if (!_lockY)
                        PercentTop = MathHelper.Clamp((Main.MouseScreen.Y - bounds.Y) / bounds.Height, 0, 1);
                }
                if (_isDraggingEndPoint)
                {
                    var bounds = Bounds;
                    if (!_lockX)
                        PercentRight = MathHelper.Clamp((Main.MouseScreen.X - bounds.X) / bounds.Width, 0, 1);
                    if (!_lockY)
                        PercentBottom = MathHelper.Clamp((Main.MouseScreen.Y - bounds.Y) / bounds.Height, 0, 1);
                }
                base.UpdateStatus(gameTime);
            }
        }

    }
}
