using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropertyPanelLibrary.Graphics2D;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using SilkyUIFramework;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Object;

public class OptionVector3 : OptionObject
{
    private Vector3Object vecObj;

    protected override void FillOption()
    {
        base.FillOption();
        PropertyPanel.Decorator = new Vector3PanelDecorator(
                    this,
                    RangeAttribute,
                    IncrementAttribute);
        if (MetaData is ListValueHandler listHandler)
            vecObj = new Vector3Object((IList<Vector3>)listHandler.List, listHandler.Index);
        else
            vecObj = new Vector3Object(MetaData.VariableInfo, MetaData.Item);
        ShowStringValueInLabel = false;
    }

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegisterOptionToType(this, typeof(Vector3));
    }

    protected override IPropertyOptionFiller GetInternalPanelFiller(object data)
    {
        return base.GetInternalPanelFiller(vecObj);
    }

    private class Vector3Object
    {
        private readonly PropertyFieldWrapper memberInfo;
        private readonly object item;
        private readonly IList<Vector3> array;
        private readonly int index;

        private Vector3 current;

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

        internal Vector3 Vec3
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

        public Vector3Object(PropertyFieldWrapper memberInfo, object item)
        {
            this.item = item;
            this.memberInfo = memberInfo;
            current = (Vector3)memberInfo.GetValue(item);
        }

        public Vector3Object(IList<Vector3> array, int index)
        {
            current = array[index];
            this.array = array;
            this.index = index;
        }
    }

    private class Vector3PanelDecorator(OptionVector3 optionVector3, RangeAttribute range, IncrementAttribute increment) : IPropertyPanelDecorator
    {
        private OptionVector3 Option { get; init; } = optionVector3;
        private RangeAttribute Range { get; init; } = range;
        private IncrementAttribute Increment { get; init; } = increment;
        private Vector3Panel VecPanel { get; set; }
        private Dimension OldWidth { get; set; }

        public void PostFillPanel(PropertyPanel panel)
        {
            var list = panel.OptionList;
            OldWidth = list.Width;
            const float height = 136;
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
            VecPanel = new Vector3Panel((Vector3)Option.GetValue(), increment, min, max);
            VecPanel.OnUpdate += delegate
            {
                var vec = (Vector3)Option.GetValue();
                VecPanel.RealValue = vec;
                VecPanel.Alpha = Option._expandTimer.Schedule;
            };
            VecPanel.PointPanel.OnUpdate += delegate
            {
                Option.vecObj.Vec3 = VecPanel.RealValue;
            };
            VecPanel.Margin = new Margin(8, 0, 8, 0);
            VecPanel.SetSize(height, 0, 0, 1);
            VecPanel.Join(panel);
        }

        public void PreFillPanel(PropertyPanel panel)
        {
        }

        public void UnloadDecorate(PropertyPanel panel)
        {
            panel.OptionList.Width = OldWidth;
            VecPanel?.RemoveFromParent();
        }

        private class Vector3Panel : UIElementGroup
        {
            public float PercentX { get; set; }
            public float PercentY { get; set; }
            public float PercentZ { get; set; }
            public UIView PointPanel { get; set; }

            public Vector3 RealValue
            {
                get
                {
                    Vector3 result = default;
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

                    value = min + (max - min) * PercentZ;
                    if (increment != 0)
                        value = MathF.Round(value / increment) * increment;
                    value = Math.Clamp(value, min, max);
                    result.Z = value;

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

                    coordValue = value.Z;
                    if (increment != 0)
                        coordValue = MathF.Round(coordValue / increment) * increment;
                    PercentZ = Utils.GetLerpValue(min, max, coordValue, true);
                }
            }

            public float Increment { get; init; }
            public float Min { get; init; }
            public float Max { get; init; }

            public float Alpha { private get; set; } = 1.0f;

            private bool _isDragging;

            private bool _draggingZ;

            private bool _draggingRotation;

            private float _rotationAssistant;

            private float RotationZ
            {
                get;
                set
                {
                    if (value == field) return;
                    field = value;
                    const float Root2Over4 = 1.414f * .25f;
                    float c = MathF.Cos(value);
                    float s = MathF.Sin(value);
                    _baseI = c * Vector2.UnitX + Root2Over4 * s * new Vector2(1, -1);
                    _baseJ = -s * Vector2.UnitX + Root2Over4 * c * new Vector2(1, -1);
                }
            } = 1;

            private Vector2 _baseI;
            private Vector2 _baseJ;
            private readonly float _step;

            public Vector3Panel(Vector3 initialValue, float increment, float min, float max)
            {
                var coordValue = initialValue.X;
                if (increment != 0)
                    coordValue = MathF.Round(coordValue / increment) * increment;
                PercentX = Utils.GetLerpValue(min, max, coordValue, true);

                coordValue = initialValue.Y;
                if (increment != 0)
                    coordValue = MathF.Round(coordValue / increment) * increment;
                PercentY = Utils.GetLerpValue(min, max, coordValue, true);

                coordValue = initialValue.Z;
                if (increment != 0)
                    coordValue = MathF.Round(coordValue / increment) * increment;
                PercentZ = Utils.GetLerpValue(min, max, coordValue, true);

                Increment = increment;
                Min = min;
                Max = max;

                BackgroundColor = Color.Black * .3f;
                BorderRadius = new Vector4(4);

                RotationZ = 0f;
                _step = Increment == 0 ? 0.2f : Increment / (Max - Min);

                PointPanel = new UIView();
                PointPanel.Join(this);
            }

            public override void OnRightMouseDown(UIMouseEvent evt)
            {
                base.OnRightMouseDown(evt);
                _draggingRotation = true;
                var bounds = Bounds;
                _rotationAssistant = MathHelper.Clamp((Main.MouseScreen.X - bounds.X) / bounds.Width, 0, 1) * MathHelper.TwoPi - RotationZ;
            }

            public override void OnRightMouseUp(UIMouseEvent evt)
            {
                base.OnRightMouseUp(evt);
                _draggingRotation = false;
            }

            public override void OnMiddleMouseClick(UIMouseEvent evt)
            {
                base.OnMiddleMouseClick(evt);
                _draggingZ = !_draggingZ;
            }

            public override void OnLeftMouseDown(UIMouseEvent evt)
            {
                base.OnLeftMouseDown(evt);
                _isDragging = true;
            }

            public override void OnLeftMouseUp(UIMouseEvent evt)
            {
                base.OnLeftMouseUp(evt);
                _isDragging = false;
            }

            protected override void UpdateStatus(GameTime gameTime)
            {
                if (_isDragging)
                {
                    var bounds = Bounds;
                    var coord = Vector2.Clamp((Main.MouseScreen - bounds.Position) / (Vector2)bounds.Size, default, Vector2.One) * 2 - Vector2.One;
                    coord *= 1.5f;
                    if (_draggingZ)
                    {
                        coord -= PercentX * _baseI + PercentY * _baseJ;
                        PercentZ = 1 - Utils.GetLerpValue(-1.0f, 1.0f, coord.Y, true);
                        //Main.NewText(coord);
                    }
                    else
                    {
                        coord -= (PercentZ * 2 - 1) * new Vector2(0, -1);
                        float det = _baseI.X * _baseJ.Y - _baseI.Y * _baseJ.X;
                        PercentX = Utils.GetLerpValue(-1, 1, (_baseJ.Y * coord.X - _baseJ.X * coord.Y) / det, true);
                        PercentY = Utils.GetLerpValue(-1, 1, (-_baseI.Y * coord.X + _baseI.X * coord.Y) / det, true);
                    }
                }
                if (_draggingRotation)
                {
                    var bounds = Bounds;
                    RotationZ = MathHelper.Clamp((Main.MouseScreen.X - bounds.X) / bounds.Width, 0, 1) * MathHelper.TwoPi - _rotationAssistant;
                }
                base.UpdateStatus(gameTime);
            }

            protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
            {
                base.Draw(gameTime, spriteBatch);

                float alpha = Alpha;

                var matrix = SDFGraphics.GetMatrix(true);
                var bounds = Bounds;
                Vector2 size = bounds.Size;
                Vector2 center = bounds.Center;
                Vector2 i = _baseI * size * .33f;
                Vector2 j = _baseJ * size * .33f;
                Vector2 k = new Vector2(0, size.Y * .33f);

                SDFGraphics.NoBorderTriangle(center + i + j + k, center - i - j + k, center - i + j + k, Color.Black * (.2f * alpha), matrix);
                SDFGraphics.NoBorderTriangle(center + i + j + k, center - i - j + k, center + i - j + k, Color.Black * (.2f * alpha), matrix);

                SDFGraphics.NoBorderLine(center, center + k, 2, Color.MediumPurple * (.5f * alpha), matrix);
                SDFGraphics.NoBorderLine(center + i + j - k, center + i + j + k, 1, Color.Black * (.1f * alpha), matrix);
                SDFGraphics.NoBorderLine(center + i - j - k, center + i - j + k, 1, Color.Black * (.1f * alpha), matrix);
                SDFGraphics.NoBorderLine(center - i + j - k, center - i + j + k, 1, Color.Black * (.1f * alpha), matrix);
                SDFGraphics.NoBorderLine(center - i - j - k, center - i - j + k, 1, Color.Black * (.1f * alpha), matrix);

                SDFGraphics.NoBorderTriangle(center + i + j, center - i - j, center - i + j, Color.Black * (.4f * alpha), matrix);
                SDFGraphics.NoBorderTriangle(center + i + j, center - i - j, center + i - j, Color.Black * (.4f * alpha), matrix);

                SDFGraphics.NoBorderLine(center, center - k, 2, Color.MediumPurple * (.5f * alpha), matrix);
                SDFGraphics.NoBorderLine(center - k, center - k + new Vector2(0.05f) * size, 2, Color.MediumPurple * (.5f * alpha), matrix);
                SDFGraphics.NoBorderLine(center - k, center - k + new Vector2(-0.05f, 0.05f) * size, 2, Color.MediumPurple * (.5f * alpha), matrix);

                //for (float s = 0; s <= 1; s += _step)
                //{
                //    Vector2 offset = j * (2 * s - 1);
                //    SDFGraphics.NoBorderLine(center - i + offset, center + i + offset, 1, Color.White * .05f, matrix);
                //    offset = i * (2 * s - 1);
                //    SDFGraphics.NoBorderLine(center - j + offset, center + j + offset, 1, Color.White * .05f, matrix);
                //}
                SDFGraphics.NoBorderLine(center - i, center + i, 2, Color.Red * (.5f * alpha), matrix);
                SDFGraphics.NoBorderLine(center + i * .85f + j * .15f, center + i, 2, Color.Red * (.5f * alpha), matrix);
                SDFGraphics.NoBorderLine(center + i * .85f - j * .15f, center + i, 2, Color.Red * (.5f * alpha), matrix);
                // SDFGraphics.NoBorderLine(center - i, center + i, 2, Color.Red * .5f, matrix);

                SDFGraphics.NoBorderLine(center - j, center + j, 2, Color.Green * (.5f * alpha), matrix);
                SDFGraphics.NoBorderLine(center + j * .85f + i * .15f, center + j, 2, Color.Green * (.5f * alpha), matrix);
                SDFGraphics.NoBorderLine(center + j * .85f - i * .15f, center + j, 2, Color.Green * (.5f * alpha), matrix);

                Vector3 kCoord = new Vector3(PercentX, PercentY, PercentZ) * 2 - Vector3.One;
                Vector2 pointCoord = center + kCoord.X * i + kCoord.Y * j - kCoord.Z * k;
                SDFGraphics.NoBorderLine(pointCoord - k + kCoord.Z * k, pointCoord + k + kCoord.Z * k, 0.5f, Color.MediumPurple * alpha, matrix);
                SDFGraphics.NoBorderLine(pointCoord - i - kCoord.X * i, pointCoord + i - kCoord.X * i, 0.5f, Color.Red * alpha, matrix);
                SDFGraphics.NoBorderLine(pointCoord - j - kCoord.Y * j, pointCoord + j - kCoord.Y * j, 0.5f, Color.Green * alpha, matrix);
                if (_draggingZ)
                {
                    Vector2 zEnd = pointCoord - k + kCoord.Z * k;
                    SDFGraphics.NoBorderLine(zEnd + size * new Vector2(0.05f), zEnd, 0.5f, Color.MediumPurple * alpha, matrix);
                    SDFGraphics.NoBorderLine(zEnd + size * new Vector2(-0.05f, 0.05f), zEnd, 0.5f, Color.MediumPurple * alpha, matrix);
                }
                else
                {
                    Vector2 xEnd = pointCoord + i - kCoord.X * i;
                    Vector2 yEnd = pointCoord + j - kCoord.Y * j;

                    SDFGraphics.NoBorderLine(xEnd - i * 0.15f + j * 0.15f, xEnd, 0.5f, Color.Red * alpha, matrix);
                    SDFGraphics.NoBorderLine(xEnd - i * 0.15f - j * 0.15f, xEnd, 0.5f, Color.Red * alpha, matrix);

                    SDFGraphics.NoBorderLine(yEnd - j * 0.15f + i * 0.15f, yEnd, 0.5f, Color.Green * alpha, matrix);
                    SDFGraphics.NoBorderLine(yEnd - j * 0.15f - i * 0.15f, yEnd, 0.5f, Color.Green * alpha, matrix);
                    // SDFGraphics.NoBorderLine(pointCoord - j - kCoord.Y * j, pointCoord + j - kCoord.Y * j, 0.5f, Color.Green, matrix);
                }

                SDFGraphics.NoBorderRound(pointCoord - new Vector2(4f), default, 8, Color.Yellow * alpha, matrix);

                SDFGraphics.NoBorderTriangle(center + i + j - k, center - i - j - k, center - i + j - k, Color.Black * (.2f * alpha), matrix);
                SDFGraphics.NoBorderTriangle(center + i + j - k, center - i - j - k, center + i - j - k, Color.Black * (.2f * alpha), matrix);
            }
        }
    }
}