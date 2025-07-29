using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropertyPanelLibrary.Graphics2D;
using SilkyUIFramework;
using SilkyUIFramework.BasicElements;
using Terraria.Audio;
using Terraria.ID;

namespace PropertyPanelLibrary.BasicElements;

public class SUITriangleIcon:UIView
{
    public float TriangleBorder;
    public Color TriangleBorderColor, TriangleBorderHoverColor, TriangleBeginColor, TriangleEndColor;
    public Vector2[] trianglePercentCoord;

    public SUITriangleIcon()
    {
        SetSize(25f, 25f);
        trianglePercentCoord = [new(0.3f, 0.0f), new(0.8f, 0.5f), new(0.3f, 1.0f)];
        TriangleBeginColor = SUIColor.Warn * 0.5f;
        TriangleEndColor = SUIColor.Warn;
        TriangleBorder = 2f;
        TriangleBorderColor = SUIColor.Border;
        TriangleBorderHoverColor = SUIColor.Highlight;

        // MarginRight = 2f;
        BorderRadius = new Vector4(8f);
        Border = 2;
        BorderColor = SUIColor.Border;
    }

    public override void OnMouseEnter(UIMouseEvent evt)
    {
        base.OnMouseEnter(evt);
        SoundEngine.PlaySound(SoundID.MenuTick);
    }
    protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        var borderColor = HoverTimer.Lerp(TriangleBorderColor, TriangleBorderHoverColor);
        base.Draw(gameTime, spriteBatch);
        Vector2 pos = Bounds.Position;
        Vector2 size = Bounds.Size;
        Color fork = HoverTimer.Lerp(TriangleBeginColor, TriangleEndColor);

        SDFGraphics.HasBorderTriangle(
            pos + size * trianglePercentCoord[0],
            pos + size * trianglePercentCoord[1], 
            pos + size * trianglePercentCoord[2],
            fork,
            TriangleBorder, 
            borderColor, 
            SDFGraphics.GetMatrix(true));
    }
}
