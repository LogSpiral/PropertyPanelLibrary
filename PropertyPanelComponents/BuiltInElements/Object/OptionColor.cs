using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropertyPanelLibrary.Graphics2D;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.OptionDecorators;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using ReLogic.Content;
using ReLogic.OS;
using SilkyUIFramework;
using SilkyUIFramework.BasicComponents;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Object;

public class OptionColor : OptionObject
{
    private static Asset<Effect> ColorPanelEffect { get; } = ModAsset.ColorPanels;

    private struct ColorPanelVertex(Vector2 pos, Vector2 coord) : IVertexType
    {
        private static readonly VertexDeclaration _vertexDeclaration = new(
        [
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        ]);

        public Vector2 Pos = pos;
        public Vector2 Coord = coord;
        public readonly VertexDeclaration VertexDeclaration => _vertexDeclaration;
    }

    private class ColorHandler
    {
        private readonly PropertyFieldWrapper memberInfo;
        private readonly object item;
        private readonly IList<Color> array;
        private readonly int index;
        private int hslImmuneCount;//用来锁hsl防止编辑它们的时候因为另外四个的变化函数又间接编辑到它们
        internal Color current;
        internal Vector3 hsl;
        internal List<OptionSlider> sliders = [];

        [LabelKey("$Config.Color.Red.Label")]
        public byte Red
        {
            get => current.R;
            set
            {
                current.R = value;
                UpdateHSL();
                Update();
            }
        }

        [LabelKey("$Config.Color.Green.Label")]
        public byte Green
        {
            get => current.G;
            set
            {
                current.G = value;
                UpdateHSL();
                Update();
            }
        }

        [LabelKey("$Config.Color.Blue.Label")]
        public byte Blue
        {
            get => current.B;
            set
            {
                current.B = value;
                UpdateHSL();
                Update();
            }
        }

        [LabelKey("$Config.Color.Hue.Label")]
        public float Hue
        {
            get => hsl.X;
            set
            {
                byte a = Alpha;
                current = Main.hslToRgb(value, Saturation, Lightness);
                current.A = a;
                Update();
                hslImmuneCount = 4;
                hsl.X = value;
            }
        }

        [LabelKey("$Config.Color.Saturation.Label")]
        public float Saturation
        {
            get => hsl.Y;
            set
            {
                byte a = Alpha;
                current = Main.hslToRgb(Hue, value, Lightness);
                current.A = a;
                Update();
                hslImmuneCount = 4;
                hsl.Y = value;
            }
        }

        [LabelKey("$Config.Color.Lightness.Label")]
        public float Lightness
        {
            get => hsl.Z;
            set
            {
                byte a = Alpha;
                current = Main.hslToRgb(Hue, Saturation, value);
                current.A = a;
                Update();
                hslImmuneCount = 4;
                hsl.Z = value;
            }
        }

        [LabelKey("$Config.Color.Alpha.Label")]
        public byte Alpha
        {
            get => current.A;
            set
            {
                current.A = value;
                Update();
            }
        }

        public string Hex
        {
            get => current.Hex3();
            set
            {
                if (uint.TryParse(value, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var result))
                {
                    uint b = result & 0xFFu;
                    uint g = (result >> 8) & 0xFFu;
                    uint r = (result >> 16) & 0xFFu;
                    current.R = (byte)r;
                    current.G = (byte)g;
                    current.B = (byte)b;
                    for (int n = 0; n < 3; n++)
                        UpdateHSL();
                    //current.packedValue = result;
                    Update();
                }
            }
        }

        internal Color CurrentColor
        {
            get => current;
            set
            {
                current = value;
                UpdateHSL();
                Update();
            }
        }

        private void Update()
        {
            if (array == null)
                memberInfo.SetValue(item, current);
            else
                array[index] = current;
            foreach (var s in sliders)
                s.SetColorPendingModified();
        }

        private void UpdateHSL()
        {
            if (hslImmuneCount-- >= 0) return;
            Vector3 neoHsl = Main.rgbToHsl(current);
            if (neoHsl.Z != 0 && neoHsl.Z != 1) //只有亮度不为1或0时剩下两个才有意义
            {
                hsl.Y = neoHsl.Y;
                if (neoHsl.Y != 0) //只有饱和度不为0色调才有意义
                    hsl.X = neoHsl.X;
            }
            hsl.Z = neoHsl.Z;
        }

        public ColorHandler(PropertyFieldWrapper memberInfo, object item)
        {
            this.item = item;
            this.memberInfo = memberInfo;
            current = (Color)memberInfo.GetValue(item);
        }

        public ColorHandler(IList<Color> array, int index)
        {
            current = array[index];
            this.array = array;
            this.index = index;
        }

        public ColorHandler(Color color) //仅用于设置单项默认值
        {
            current = color;
            hsl = Main.rgbToHsl(color);
        }
    }

    private static void DrawRGBPanel(Vector2 pos, Vector2 size, Color current)
    {
        ColorPanelVertex[] vertexs = new ColorPanelVertex[4];
        for (int n = 0; n < 4; n++)
        {
            Vector2 coord = new Vector2(n / 2, n % 2);
            vertexs[n] = new ColorPanelVertex(pos + size * coord, coord);
        }
        Effect colorPanel = ColorPanelEffect.Value;
        colorPanel.Parameters["uColor"].SetValue(current.ToVector3());
        colorPanel.Parameters["uTransform"].SetValue(SDFGraphics.GetMatrix(true));
        colorPanel.CurrentTechnique.Passes[0].Apply();
        Main.instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, [vertexs[0], vertexs[1], vertexs[2], vertexs[1], vertexs[2], vertexs[3]], 0, 2);
        Main.spriteBatch.spriteEffectPass.Apply();
    }

    private static void DrawHSLRing(Vector2 pos, Vector2 size, Vector3 hsl)
    {
        ColorPanelVertex[] vertexs = new ColorPanelVertex[4];
        for (int n = 0; n < 4; n++)
        {
            Vector2 coord = new Vector2(n / 2, n % 2);
            vertexs[n] = new ColorPanelVertex(pos + size * coord, coord);
        }
        Effect colorPanel = ColorPanelEffect.Value;
        colorPanel.Parameters["uHsl"].SetValue(hsl);
        colorPanel.Parameters["uHueRotation"].SetValue(Matrix.CreateRotationZ(hsl.X * MathHelper.TwoPi));
        colorPanel.Parameters["uTransform"].SetValue(SDFGraphics.GetMatrix(true));
        colorPanel.CurrentTechnique.Passes[1].Apply();
        Main.instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, [vertexs[0], vertexs[1], vertexs[2], vertexs[1], vertexs[2], vertexs[3]], 0, 2);
        Main.spriteBatch.spriteEffectPass.Apply();
    }

    private class SlidePreviewButton : UIView
    {
        protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            var bounds = Bounds;
            SDFRectangle.BarColor(
                bounds.Position,
                bounds.Size,
                BorderRadius,
                TextureAssets.Extra[180].Value,
                Vector2.UnitX * .05f,
                0,
                Main.UIScaleMatrix);

            SDFRectangle.HasBorder(
                bounds.Position,
                bounds.Size,
                BorderRadius,
                default,
                2,
                HoverTimer.Lerp(default, SUIColor.Highlight),
                Main.UIScaleMatrix);
        }
    }

    private class RGBPanel : UIElementGroup
    {
        private OptionColor Option { get; set; }
        private UIView RedSlider { get; set; }
        private UIView GreenBlueRound { get; set; }
        private bool _dragging;
        private bool _draggingRed;

        public RGBPanel(OptionColor optionColor)
        {
            Option = optionColor;
            LayoutType = LayoutType.Custom;
            RedSlider = new UIView()
            {
                Height = new(0, .1f),
                Width = new(6, 0),
                BackgroundColor = Color.Transparent,
                Border = 1,
                BorderColor = Color.Black
            };
            RedSlider.OnUpdate += delegate
            {
                float factor = Option.currentColor.R / 255f;
                RedSlider.SetLeft(factor * -6f, MathHelper.Lerp(0.2f, 0.8f, factor));
                RedSlider.SetTop(0, 0.8f);
                RedSlider.BorderColor = RedSlider.HoverTimer.Lerp(Color.Black, SUIColor.Highlight);
            };
            RedSlider.Join(this);
            GreenBlueRound = new UIView()
            {
                Height = new(8, 0),
                Width = new(8, 0),
                BackgroundColor = Color.Transparent,
                Border = 1,
                BorderColor = Color.Black,
                BorderRadius = new(4)
            };
            GreenBlueRound.OnUpdate += delegate
            {
                GreenBlueRound.SetLeft(0, MathHelper.Lerp(0.2f, 0.8f, Option.currentColor.G / 255f) - .5f, .5f);
                GreenBlueRound.SetTop(0, MathHelper.Lerp(0.1f, 0.7f, Option.currentColor.B / 255f) - .5f, .5f);
                GreenBlueRound.BorderColor = GreenBlueRound.HoverTimer.Lerp(Color.Black, SUIColor.Highlight);
            };
            GreenBlueRound.Join(this);
        }

        public override void OnLeftMouseDown(UIMouseEvent evt)
        {
            // if (evt.Source != this) return;
            var bounds = Bounds;
            Vector2 v = Main.MouseScreen - bounds.Position;
            v /= (Vector2)bounds.Size;
            if (MathF.Abs(v.X - .5f) < .3f)
            {
                if (MathF.Abs(v.Y - .4f) < .3f)
                {
                    _dragging = true;
                    _draggingRed = false;
                }
                if (MathF.Abs(v.Y - .85f) < .05f)
                {
                    _dragging = true;
                    _draggingRed = true;
                }
            }
            base.OnLeftMouseDown(evt);
        }

        public override void OnLeftMouseUp(UIMouseEvent evt)
        {
            _dragging = false;
            base.OnLeftMouseUp(evt);
        }

        protected override void UpdateStatus(GameTime gameTime)
        {
            base.UpdateStatus(gameTime);
            if (!_dragging) return;
            var bounds = Bounds;
            Vector2 v = Main.MouseScreen - bounds.Position;
            v /= (Vector2)bounds.Size;
            if (_draggingRed)
                Option._colorHandler.Red = (byte)(255 * MathHelper.Clamp((v.X - .2f) / .6f, 0, 1));
            else
            {
                Option._colorHandler.Green = (byte)(255 * MathHelper.Clamp((v.X - .2f) / .6f, 0, 1));
                Option._colorHandler.Blue = (byte)(255 * MathHelper.Clamp((v.Y - .1f) / .6f, 0, 1));
            }
        }

        protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            DrawRGBPanel(Bounds.Position, Bounds.Size, Option.currentColor);
        }
    }

    private class HSLRing : UIElementGroup
    {
        private OptionColor Option { get; set; }
        private UIView HueSlder { get; set; }
        private UIView SaturationLightnessRound { get; set; }
        private bool _dragging;
        private bool _draggingHue;

        public HSLRing(OptionColor optionColor)
        {
            Option = optionColor;
            LayoutType = LayoutType.Custom;
            HueSlder = new UIView()
            {
                Height = new(0, .1f),
                Width = new(0, .1f),
                BackgroundColor = Color.Transparent,
                Border = 1,
                BorderColor = Color.Black
            };
            HueSlder.OnUpdate += delegate
            {
                var hsl = Option.currentHSL;
                HueSlder.SetLeft(0, MathF.Cos(hsl.X * MathHelper.TwoPi) * .37f, .5f);
                HueSlder.SetTop(0, MathF.Sin(hsl.X * MathHelper.TwoPi) * .37f, .5f);
                HueSlder.BorderColor = HueSlder.HoverTimer.Lerp(Color.Black, SUIColor.Highlight);
                Vector2 size = HueSlder.Bounds.Size;
                size *= .5f;
                HueSlder.BorderRadius = new(size, size.X, size.Y);
            };
            HueSlder.Join(this);
            SaturationLightnessRound = new UIView()
            {
                Height = new(8, 0),
                Width = new(8, 0),
                BackgroundColor = Color.Transparent,
                Border = 1,
                BorderColor = Color.Black,
                BorderRadius = new(4)
            };
            SaturationLightnessRound.OnUpdate += delegate
            {
                var hsl = Option.currentHSL;
                var vec = new Vector2(hsl.Y * .6f - .3f, (hsl.Z * .6f - .3f) * (1 - 2 * MathF.Abs(hsl.Y - .5f)));
                vec = Vector2.Transform(vec, Matrix.CreateRotationZ(hsl.X * MathHelper.TwoPi));

                SaturationLightnessRound.SetLeft(0, vec.X, .5f);
                SaturationLightnessRound.SetTop(0, vec.Y, .5f);
                SaturationLightnessRound.BorderColor = SaturationLightnessRound.HoverTimer.Lerp(Color.Black, SUIColor.Highlight);
            };
            SaturationLightnessRound.Join(this);
        }

        public override void OnLeftMouseDown(UIMouseEvent evt)
        {
            // if (evt.Source != this) return;
            var bounds = Bounds;
            Vector2 v = Main.MouseScreen - bounds.Position;
            v /= (Vector2)bounds.Size;
            v -= new Vector2(.5f);
            float ls = v.LengthSquared();
            if (ls > 0.09 && ls < 0.16)
            {
                _dragging = true;
                _draggingHue = true;
            }
            v = Vector2.Transform(v, Matrix.CreateRotationZ(Option.currentHSL.X * MathHelper.TwoPi));
            if (MathF.Abs(v.X) + MathF.Abs(v.Y) < 0.3)
            {
                _dragging = true;
                _draggingHue = false;
            }
            base.OnLeftMouseDown(evt);
        }

        public override void OnLeftMouseUp(UIMouseEvent evt)
        {
            _dragging = false;
            base.OnLeftMouseUp(evt);
        }

        protected override void UpdateStatus(GameTime gameTime)
        {
            base.UpdateStatus(gameTime);
            if (!_dragging) return;
            var bounds = Bounds;
            Vector2 v = Main.MouseScreen - bounds.Position;
            v /= (Vector2)bounds.Size;
            v -= new Vector2(.5f);
            float ls = v.LengthSquared();
            if (_draggingHue)
                Option._colorHandler.Hue = (v.ToRotation() + 6.283f) % 6.283f / 6.283f;
            else
            {
                v = Vector2.Transform(v, Matrix.CreateRotationZ(-Option.currentHSL.X * MathHelper.TwoPi));
                Option._colorHandler.Saturation = MathHelper.Clamp(v.X / .6f + .5f, 0.01f, 1);
                Option._colorHandler.Lightness = MathHelper.Clamp(v.Y / (.3f - MathF.Abs(v.X)) * .5f + .5f, 0.01f, 0.99f);//0或1会出现*坍缩*
            }
        }

        protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            DrawHSLRing(Bounds.Position, Bounds.Size, Option.currentHSL);
        }
    }

    private ColorHandler _colorHandler;
    private Color currentColor;
    private Vector3 currentHSL;
    private SUIImage ModeButton { get; set; }
    private SlidePreviewButton PreviewButton { get; set; }
    private bool _previewMode;
    private int _currentMode { get; set; }

    protected override IPropertyOptionFiller GetInternalPanelFiller(object data)
    {
        return _currentMode switch
        {
            0 => new DesignatedMemberFiller(
                (_colorHandler,
                [
                    nameof(ColorHandler.Red),
                    nameof(ColorHandler.Green),
                    nameof(ColorHandler.Blue),
                    nameof(ColorHandler.Alpha)
                ])).SetAsSubOption(this),
            1 => new DesignatedMemberFiller(
                (_colorHandler,
                [
                    nameof(ColorHandler.Hue),
                    nameof(ColorHandler.Saturation),
                    nameof(ColorHandler.Lightness),
                    nameof(ColorHandler.Alpha)
                ])).SetAsSubOption(this),
            2 or _ => new ObjectMetaDataFiller(_colorHandler).SetAsSubOption(this, MetaData.Item, MetaData.VariableInfo)
        };
    }

    protected override void FillOption()
    {
        base.FillOption();

        PropertyPanel.OptionDecorator =
            new CombinedOptionDecorator(
                LabelOptionDecorator.NewLabelDecorator(),
                new ColorPreviewDecorator(this)
                );
        PropertyPanel.Decorator = new RGBPanelDecorator(this);

        UIElementGroup colorPVPanel = new();
        colorPVPanel.SetSize(90, 25);
        colorPVPanel.SetMargin(4, 0);
        colorPVPanel.BorderRadius = new(10);
        colorPVPanel.Join(ButtonContainer);
        UITextView popupText = new();
        popupText.Join(colorPVPanel);
        popupText.TextAlign = new(0, .5f);
        popupText.SetTop(0, 0, .5f);
        popupText.SetLeft(4);
        colorPVPanel.OnUpdate += delegate
        {
            colorPVPanel.BackgroundColor = currentColor;
            if (!colorPVPanel.IsMouseHovering)
                popupText.Text = "";
        };

        colorPVPanel.LeftMouseClick += delegate
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            string code = currentColor.Hex3();
            Platform.Get<IClipboard>().Value = code;
            popupText.Text = "复制了喵";//;Language.GetTextValue("PropertyPanel.BuiltInElements.OptionColor.ColorCopiedPopup");
            popupText.TextColor = Color.White;
        };
        colorPVPanel.RightMouseClick += delegate
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            var str = Platform.Get<IClipboard>().Value;
            if (uint.TryParse(str, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var result))
            {
                //currentColor.packedValue = result;
                _colorHandler.Hex = str;
                popupText.Text = "粘贴了喵"; //Language.GetTextValue("PropertyPanel.BuiltInElements.OptionColor.ColorPastedPopup");
                popupText.TextColor = Color.White;
            }
        };

        PreviewButton = new();
        PreviewButton.SetSize(25, 25);
        PreviewButton.SetMargin(4, 0);
        PreviewButton.BorderRadius = new(8f);
        PreviewButton.BackgroundColor = Color.Black * .2f;
        PreviewButton.LeftMouseClick += delegate
        {
            _previewMode = !_previewMode;
            foreach (var slider in _colorHandler.sliders)
                slider.SetColorPendingModified();
            SoundEngine.PlaySound(SoundID.MenuTick);
        };
        PreviewButton.Join(ButtonContainer);
        PreviewButton.OnUpdate += delegate
        {
            currentColor = _colorHandler.current with { A = 255 };
            currentHSL = _colorHandler.hsl;

            float factor = _expandTimer.Schedule;
            PreviewButton.SetMargin(4 * factor, 0);
            PreviewButton.SetWidth(25 * factor, 0);
        };

        ModeButton = new(ModAsset.RGBMode);
        ModeButton.SetSize(25, 25);
        ModeButton.SetMargin(4, 0);
        ModeButton.BackgroundColor = default;
        ModeButton.FitHeight = false;
        ModeButton.FitWidth = false;
        ModeButton.BorderRadius = new(8f);
        ModeButton.ImageScale = new(.67f);
        ModeButton.ImageAlign = new(.5f);
        ModeButton.Join(ButtonContainer);
        ModeButton.LeftMouseClick += delegate
        {
            pendingChanges = true;
            _colorHandler.sliders.Clear();
            _currentMode++;
            _currentMode %= 3;
            ModeButton.Texture2D = _currentMode switch
            {
                0 => ModAsset.RGBMode,
                1 => ModAsset.HSLMode,
                2 or _ => ModAsset.PanelMode
            };
            //PropertyPanel.Decorator = new CombinedDecorator(
            //                            FitHeightDecorator.Instance,
            //                            _currentMode switch
            //                            {
            //                                0 => new RGBPanelDecorator(this),
            //                                1 => new HSLRingDecorator(this),
            //                                2 or _ => new BothPanelDecorator(this)
            //                            });
            PropertyPanel.Decorator = _currentMode switch
            {
                0 => new RGBPanelDecorator(this),
                1 => new HSLRingDecorator(this),
                2 or _ => new BothPanelDecorator(this)
            };
        };
        ModeButton.OnUpdate += delegate
        {
            float factor = _expandTimer.Schedule;
            ModeButton.SetPadding(4 * factor, 0);
            ModeButton.SetWidth(25 * factor, 0);
            ModeButton.ImageScale = new Vector2(factor, 1) * .67f;
        };
        if (MetaData is ListValueHandler listHandler)
            _colorHandler = new((IList<Color>)listHandler.List, listHandler.Index);
        else
            _colorHandler = new(MetaData.VariableInfo, MetaData.Item);
        ShowStringValueInLabel = false;
    }

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegisterOptionToType(this, typeof(Color));
    }

    private class ColorPreviewDecorator(OptionColor option) : IPropertyOptionDecorator
    {
        private OptionColor Option { get; set; } = option;

        IPropertyOptionDecorator IPropertyOptionDecorator.Clone() => this;

        void IPropertyOptionDecorator.PostFillOption(PropertyOption option)
        {
            if (option is not OptionSlider slider) return;
            slider.SetColorMethod(option.MetaData.VariableInfo.Name switch
            {
                "Red" => t => Option._previewMode ? Option.currentColor with { A = 255, R = (byte)(255 * t) } : Color.Black * 0.3f,
                "Green" => t => Option._previewMode ? Option.currentColor with { A = 255, G = (byte)(255 * t) } : Color.Black * 0.3f,
                "Blue" => t => Option._previewMode ? Option.currentColor with { A = 255, B = (byte)(255 * t) } : Color.Black * 0.3f,
                "Alpha" => t => Option._previewMode ? Option.currentColor with { A = 255 } * t : Color.Black * 0.3f,
                "Hue" => t => Option._previewMode ? Main.hslToRgb(Option.currentHSL with { X = t }) : Color.Black * 0.3f,
                "Saturation" => t => Option._previewMode ? Main.hslToRgb(Option.currentHSL with { Y = t }) : Color.Black * 0.3f,
                "Lightness" or _ => t => Option._previewMode ? Main.hslToRgb(Option.currentHSL with { Z = t }) : Color.Black * 0.3f,
            });
            Option._colorHandler.sliders.Add(slider);
        }

        void IPropertyOptionDecorator.PreFillOption(PropertyOption option)
        {
        }

        void IPropertyOptionDecorator.UnloadDecorate(PropertyOption option)
        {
            if (option is not OptionSlider slider) return;
            slider.SetColorMethod(null);
            Option._colorHandler.sliders.Remove(slider);
        }
    }

    private class RGBPanelDecorator(OptionColor option) : IPropertyPanelDecorator
    {
        private OptionColor Option { get; init; } = option;
        private RGBPanel RGBPanel { get; set; }
        private Dimension OldWidth { get; set; }

        public void PostFillPanel(PropertyPanel panel)
        {
            var list = panel.OptionList;
            OldWidth = list.Width;
            const float height = 184;
            list.SetWidth(-height - 8, 1);

            RGBPanel = new(Option);
            RGBPanel.Margin = new(8, 0, 8, 0);
            RGBPanel.SetSize(height, 0, 0, 1);
            RGBPanel.BorderRadius = new(8f);
            RGBPanel.BackgroundColor = Color.Black * .4f;
            RGBPanel.Join(panel);
        }

        public void PreFillPanel(PropertyPanel panel)
        {
        }

        public void UnloadDecorate(PropertyPanel panel)
        {
            panel.OptionList.Width = OldWidth;
            RGBPanel?.Remove();
        }
    }

    private class HSLRingDecorator(OptionColor option) : IPropertyPanelDecorator
    {
        private OptionColor Option { get; init; } = option;
        private HSLRing HSLRing { get; set; }
        private Dimension OldWidth { get; set; }

        public void PostFillPanel(PropertyPanel panel)
        {
            var list = panel.OptionList;
            OldWidth = list.Width;
            const float height = 184;
            list.SetWidth(-height - 8, 1);

            HSLRing = new(Option);
            HSLRing.Margin = new(8, 0, 8, 0);
            HSLRing.SetSize(height, 0, 0, 1);
            HSLRing.BorderRadius = new(8f);
            HSLRing.BackgroundColor = Color.Black * .4f;
            HSLRing.Join(panel);
        }

        public void PreFillPanel(PropertyPanel panel)
        {
        }

        public void UnloadDecorate(PropertyPanel panel)
        {
            panel.OptionList.Width = OldWidth;
            HSLRing?.Remove();
        }
    }

    private class BothPanelDecorator(OptionColor option) : IPropertyPanelDecorator
    {
        private OptionColor Option { get; init; } = option;
        private RGBPanel RGBPanel { get; set; }
        private HSLRing HSLRing { get; set; }
        private UIElementGroup PanelMask { get; set; }
        private Dimension OldWidth { get; set; }

        public void PostFillPanel(PropertyPanel panel)
        {
            var list = panel.OptionList;
            OldWidth = list.Width;
            const float height = 100;
            list.SetWidth(-height - 8, 1);

            PanelMask = new();
            PanelMask.SetSize(height, 0, 0, 1);
            PanelMask.FlexWrap = true;
            PanelMask.FlexDirection = FlexDirection.Column;
            PanelMask.MainAlignment = MainAlignment.SpaceBetween;
            PanelMask.Join(panel);

            RGBPanel = new(Option);
            RGBPanel.Margin = new(8, 0, 8, 0);
            RGBPanel.SetSize(height, -4, 0, .5f);
            RGBPanel.BorderRadius = new(8f);
            RGBPanel.BackgroundColor = Color.Black * .4f;
            RGBPanel.Join(PanelMask);

            HSLRing = new(Option);
            HSLRing.Margin = new(8, 0, 8, 0);
            HSLRing.SetSize(height, -4, 0, .5f);
            HSLRing.BorderRadius = new(8f);
            HSLRing.BackgroundColor = Color.Black * .4f;
            HSLRing.Join(PanelMask);
        }

        public void PreFillPanel(PropertyPanel panel)
        {
        }

        public void UnloadDecorate(PropertyPanel panel)
        {
            panel.OptionList.Width = OldWidth;
            PanelMask?.Remove();
        }
    }
}