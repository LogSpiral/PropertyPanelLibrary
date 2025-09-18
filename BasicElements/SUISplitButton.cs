using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropertyPanelLibrary.Graphics2D;
using SilkyUIFramework;
using SilkyUIFramework.Animation;
using SilkyUIFramework.Elements;
using Terraria;

namespace PropertyPanelLibrary.BasicElements;

public class SUISplitButton : UIView
{
    private AnimationTimer _upButtonTimer = new(3);
    private AnimationTimer _downButtonTimer = new(3);

    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);
        // 更新Timer
        _upButtonTimer.Update(gameTime);
        _downButtonTimer.Update(gameTime);

        if (IsUP)
            _upButtonTimer.StartUpdate();
        else
            _upButtonTimer.StartReverseUpdate();

        if (IsDown)
            _downButtonTimer.StartUpdate();
        else
            _downButtonTimer.StartReverseUpdate();
    }

    protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(gameTime, spriteBatch);
        var pos = Bounds.Position;
        var sizeS = Bounds.Size;
        var size = new Vector2(sizeS.Width, sizeS.Width);
        float h = (pos + size * Vector2.UnitY * .25f).Y;
        var flip = Matrix.CreateTranslation(0, -h, 0) * Matrix.CreateScale(1, -1, 1) * Matrix.CreateTranslation(0, h, 0);

        Color upColor = _upButtonTimer.Lerp(SUISlideBox.SliderRound, SUISlideBox.SliderRoundHover);
        SDFGraphics.HasBorderTriangleIsosceles(pos + size * Vector2.UnitY * .05f + size * Vector2.UnitX * .5f, new Vector2(.5f, 0), size * new Vector2(.75f, .4f), buttonColor, 1, upColor, flip * SDFGraphics.GetMatrix(true));

        Color downColor = _downButtonTimer.Lerp(SUISlideBox.SliderRound, SUISlideBox.SliderRoundHover);
        SDFGraphics.HasBorderTriangleIsosceles(pos + size * Vector2.UnitY * .55f + size * Vector2.UnitX * .5f, new Vector2(.5f, 0), size * new Vector2(.75f, .4f), buttonColor, 1, downColor, SDFGraphics.GetMatrix(true));
    }

    public Color buttonColor;
    public Color buttonBorderColor;

    // Cy修改：添加了IgnoresMouseInteraction判断，在这个属性为true时，不会响应鼠标悬停事件（不会高光边框）
    public bool IsUP => !IgnoreMouseInteraction && IsMouseHovering && Main.mouseY < Bounds.Center.Y;

    public bool IsDown => !IgnoreMouseInteraction && IsMouseHovering && Main.mouseY >= Bounds.Center.Y;

    public event MouseEventHandler LeftMouseClickUP;

    public event MouseEventHandler LeftMouseClickDown;

    public override void OnLeftMouseClick(UIMouseEvent evt)
    {
        base.OnLeftMouseClick(evt);
        if (IsUP)
            LeftMouseClickUP?.Invoke(this, evt);
        if (IsDown)
            LeftMouseClickDown?.Invoke(this, evt);
    }
}