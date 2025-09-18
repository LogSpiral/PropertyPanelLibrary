using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropertyPanelLibrary.Graphics2D;
using SilkyUIFramework;
using SilkyUIFramework.Elements;

namespace PropertyPanelLibrary.BasicElements;

public class SUIPlus : UIView
{
    public float PlusSize, PlusRounded, PlusBorder;
    public Color PlusBorderColor, PlusBorderHoverColor, PlusBeginColor, PlusEndColor;

    public Vector2 CrossOffset;

    public SUIPlus()
    {
        SetSize(25, 25);

        PlusSize = 24f;
        PlusRounded = 3.6f;
        PlusBeginColor = SUIColor.Warn * 0.5f;
        PlusEndColor = SUIColor.Warn;
        PlusBorder = 2f;
        PlusBorderColor = SUIColor.Border;
        PlusBorderHoverColor = SUIColor.Highlight;

        // MarginRight = 2f;
        BorderRadius = new Vector4(4f);
        Border = 2;
        BorderColor = SUIColor.Border;
    }

    protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        var borderColor = HoverTimer.Lerp(PlusBorderColor, PlusBorderHoverColor);
        base.Draw(gameTime, spriteBatch);
        Color fork = HoverTimer.Lerp(PlusBeginColor, PlusEndColor);

        SDFGraphics.HasBorderPlus(
            Bounds.Center,
            new Vector2(.5f),
            PlusSize * .6f,
            PlusSize * .3f,
            PlusRounded,
            fork,
            PlusBorder,
            borderColor,
            SDFGraphics.GetMatrix(true));
    }
}