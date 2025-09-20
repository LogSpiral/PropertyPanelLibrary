using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropertyPanelLibrary.Graphics2D;
using SilkyUIFramework;
using SilkyUIFramework.Elements;
using System;
using Terraria;

namespace PropertyPanelLibrary.BasicElements;

public class SUISlideBox : UIElementGroup
{
    private Vector2 MousePositionRelative => Main.MouseScreen - Bounds.Position;

    public event Action ValueChangeCallback;

    public event Action EndDraggingCallback;

    private float _value;
    private bool _dragging;

    public Utils.ColorLerpMethod ColorMethod;
    public bool colorMethodPendingModified = true;
    private Texture2D colorBar;

    public float Value
    {
        get => Math.Clamp(_value, 0, 1);
        set
        {
            float wantedValue = Math.Clamp(value, 0, 1);
            if (Math.Clamp(_value, 0, 1) != wantedValue)
            {
                _value = Math.Clamp(value, 0, 1);
                ValueChangeCallback?.Invoke();
            }
        }
    }

    public SUISlideBox()
    {
        BackgroundColor = Color.Black * 0.3f;
        // Border = 6f;
    }

    ~SUISlideBox()
    {
        Main.RunOnMainThread(() => colorBar?.Dispose());
        //colorBar?.Dispose();
    }

    public override void OnLeftMouseDown(UIMouseEvent evt)
    {
        UpdateDragging();
        _dragging = true;
    }

    public override void OnLeftMouseUp(UIMouseEvent evt)
    {
        _dragging = false;
        if (_dragging)
            EndDraggingCallback?.Invoke();
    }

    private void UpdateDragging()
    {
        var size = Bounds.Size;
        float roundRadius = size.Height / 2f - 2f; // 半径
        float roundDiameter = roundRadius * 2; // 直径
        float moveWidth = size.Width - roundDiameter; // 移动宽度
        float mousePositionX = MousePositionRelative.X;
        float adjustedMousePosX = mousePositionX - roundRadius; // 让圆心保持在鼠标位置
        Value = adjustedMousePosX / moveWidth;
    }

    public static Color SliderRound { get; } = new Color(20, 25, 60);
    public static Color SliderRoundHover { get; } = new Color(233, 176, 0);

    protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(gameTime, spriteBatch);

        // 基本字段
        var position = Bounds.Position;
        var center = Bounds.Center;
        var size = Bounds.Size;
        BorderRadius = new Vector4(Bounds.Height * .5f);
        if (ColorMethod != null)
        {
            if (colorBar == null)
            {
                try
                {
                    colorBar = new Texture2D(Main.graphics.GraphicsDevice, 300, 1);
                }
                catch
                {
                    Texture2D texdummy = null;
                    Main.RunOnMainThread(() => { texdummy = new Texture2D(Main.graphics.GraphicsDevice, 300, 1); });
                    colorBar = texdummy;
                }
            }
            if (colorMethodPendingModified)
            {
                Color[] colors = new Color[300];
                for (int n = 0; n < 300; n++)
                    colors[n] = ColorMethod.Invoke(n / 299f);
                colorBar.SetData(colors);
                colorMethodPendingModified = false;
            }
            SDFRectangle.BarColor(position, size, BorderRadius, colorBar, Vector2.UnitX / Bounds.Width, 0, Main.UIScaleMatrix);
        }
        else
            base.Draw(gameTime, spriteBatch);

        if (_dragging)
            UpdateDragging();

        // 圆的属性
        float roundRadius = size.Height / 2f - 2f; // 半径
        float roundDiameter = roundRadius * 2; // 直径
        float moveWidth = size.Width - roundDiameter; // 移动宽度
        var roundCenter = position + new Vector2(Value * moveWidth, 0f);
        roundCenter.Y = center.Y;
        roundCenter.X += roundRadius;
        var roundLeftTop = roundCenter - new Vector2(roundRadius);

        // 颜色选择
        var innerColor = ColorMethod != null ? ColorMethod.Invoke(_value) : SliderRound;
        //var borderColor = innerColor;
        var borderColor = SliderRound;
        if (SDFGraphics.MouseInRound(roundCenter, (int)roundRadius))
            borderColor = SliderRoundHover;

        // 绘制
        SDFGraphics.HasBorderRound(roundLeftTop, default, roundDiameter, innerColor, 2f, borderColor, SDFGraphics.GetMatrix(true));
    }

    public void OutSideEditEnd() => EndDraggingCallback?.Invoke();
}