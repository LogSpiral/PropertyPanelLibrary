using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilkyUIFramework;
using SilkyUIFramework.Graphics2D;
using Terraria;

namespace PropertyPanelLibrary.BasicElements;

public class SUIBlurMaskView : UIElementGroup
{
    protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(gameTime, spriteBatch);
        if (BlurMakeSystem.BlurAvailable)
        {
            var bounds = Bounds;
            var radius = BorderRadius;
            if (BlurMakeSystem.SingleBlur)
            {
                var batch = Main.spriteBatch;
                batch.End();
                BlurMakeSystem.KawaseBlur();
                batch.Begin();

                //var originalScissor = spriteBatch.GraphicsDevice.ScissorRectangle;
                //spriteBatch.End();
                //var renderStatus = RenderStates.BackupStates(Main.graphics.GraphicsDevice, spriteBatch);
                //BlurMakeSystem.KawaseBlur();
                //spriteBatch.GraphicsDevice.ScissorRectangle = originalScissor;
                //renderStatus.Begin(spriteBatch, SpriteSortMode.Deferred);
            }
            SDFRectangle.SampleVersion(BlurMakeSystem.BlurRenderTarget,
                bounds.Position * Main.UIScale, bounds.Size * Main.UIScale, radius * Main.UIScale, Matrix.Identity);
        }
    }

    public SUIBlurMaskView()
    {
        FitWidth = true;
        FitHeight = true;
        SetPadding(0);
        Margin = 0;
    }
}