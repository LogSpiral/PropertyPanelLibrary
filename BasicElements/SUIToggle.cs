using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilkyUIFramework;
using SilkyUIFramework.Animation;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Graphics2D;
using System;
using Terraria.Audio;
using Terraria.ID;

namespace PropertyPanelLibrary.BasicElements;

public class SUIToggle : UIView
{
    private readonly AnimationTimer _toggleTimer = new();

    // 边框
    public Color SwitchBorder { get; set; } = new Color(20, 25, 60);

    public Color SwitchBorderHover { get; set; } = new Color(233, 176, 0);

    // 圆
    public Color SwitchRound { get; set; } = new Color(20, 25, 60);

    public Color SwitchRoundHover { get; set; } = new Color(233, 176, 0);

    // 背景色
    public Color SwitchBg { get; set; } = new Color(20, 25, 60, 127);

    public Color SwitchBgHover { get; set; } = new Color(72, 63, 63, 127);

    public Anchor ToggleAnchorX { get; private set; }
    public Anchor ToggleAnchorY { get; private set; }

    public void SetToggleLeft(float? pixel = null, float? percent = null, float? alignment = null)
    {
        ToggleAnchorX = ToggleAnchorX.With(pixel, percent, alignment);
    }

    public void SetToggleRight(float? pixel = null, float? percent = null, float? alignment = null)
    {
        ToggleAnchorY = ToggleAnchorY.With(pixel, percent, alignment);
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        var timer = _toggleTimer;
        if (Enabled) timer.StartUpdate();
        else timer.StartReverseUpdate();
        timer.Update(gameTime);
    }

    public override void OnLeftMouseClick(UIMouseEvent evt)
    {
        base.OnLeftMouseClick(evt);
        Enabled = !Enabled;
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(gameTime, spriteBatch);
        var timer = _toggleTimer;

        Color color = Color.Lerp(SwitchBg, SwitchBgHover, timer.Schedule);
        Color color2 = Color.Lerp(SwitchBorder, SwitchBorderHover, timer.Schedule);
        Color color3 = Color.Lerp(SwitchRound, SwitchRoundHover, timer.Schedule);

        var position = Bounds.Position;
        var size = Bounds.Size;

        //if (!Interactable)
        //{
        //    color *= 0.5f;
        //    color2 = Color.Gray * 0.6f;
        //    color3 = Color.Gray * 0.6f;
        //}

        // 开关

        float adjustScaler = MathF.Min(size.Width / 48f, size.Height / 26f);

        var boxSize = new Vector2(48, 26) * adjustScaler;

        var boxPosition = position + new Vector2(ToggleAnchorX.CalculatePosition(size.Width, boxSize.X), ToggleAnchorY.CalculatePosition(size.Height, boxSize.Y));

        SDFRectangle.DrawHasBorder(boxPosition, boxSize, new Vector4(MathF.Min(boxSize.X, boxSize.Y) / 2), color, 2, color2, SilkyUI.TransformMatrix);

        const float scaler = 16f / 26f;

        Vector2 boxSize2 = new(boxSize.Y * scaler);
        Vector2 position2 = boxPosition + Vector2.Lerp(new Vector2(5 * adjustScaler, size.Height / 2 - boxSize2.Y / 2),
            new Vector2(boxSize.X - 5 * adjustScaler - boxSize2.X, size.Height / 2 - boxSize2.Y / 2), timer.Schedule);

        SDFGraphics.NoBorderRound(position2, boxSize2.Y, color3, SilkyUI.TransformMatrix);
    }

    public event EventHandler<ValueChangedEventArgs<bool>> OnContentsChanged;

    public bool Enabled
    {
        get;
        set
        {
            OnContentsChanged?.Invoke(this, new ValueChangedEventArgs<bool>(field, value));
            field = value;
        }
    }
}