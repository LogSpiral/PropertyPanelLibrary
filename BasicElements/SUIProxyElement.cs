using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilkyUIFramework;
using SilkyUIFramework.Elements;
using Terraria.UI;

// using VanillaMouseEvent = Terraria.UI.UIMouseEvent;
// using SilkyMouseEvent = SilkyUIFramework.UIMouseEvent;
namespace PropertyPanelLibrary.BasicElements;

/// <summary>
/// 用SilkyUIFramework的UIView代理UIElement
/// 目前因为技术原因和需求只能代理绘制
/// </summary>
public class SUIProxyElement : UIView
{
    public UIElement InnerElement { get; set; }

    public SUIProxyElement(UIElement innerElement, bool syncLayout = true)
    {
        InnerElement = innerElement;
        if (!syncLayout) return;

        SetLeft(innerElement.Left.Pixels, innerElement.Left.Percent, innerElement.HAlign);
        SetTop(innerElement.Top.Pixels, innerElement.Top.Percent, innerElement.VAlign);
        SetSize(innerElement.Width.Pixels, innerElement.Height.Pixels, innerElement.Width.Percent, innerElement.Height.Percent);
        Margin = new Margin(innerElement.MarginLeft, innerElement.MarginTop, innerElement.MarginRight, innerElement.MarginBottom);
        Padding = new Margin(innerElement.PaddingLeft, innerElement.PaddingTop, innerElement.PaddingRight, innerElement.PaddingBottom);
        BackgroundColor = Color.Black * .5f;
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        InnerElement._innerDimensions = InnerBounds.ToDimension();
        InnerElement._dimensions = Bounds.ToDimension();
        InnerElement._outerDimensions = Bounds.ToDimension();
        base.UpdateStatus(gameTime);
    }

    protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        InnerElement.Draw(spriteBatch);
    }

    // public override void OnLeftMouseClick(SilkyMouseEvent evt)
    // {
    //     if (evt.Source == this)
    //         InnerElement.LeftClick(evt.ToVanillaMouseEvent(InnerElement));
    //     base.OnLeftMouseClick(evt);
    // }
}

internal static class MyUtils
{
    public static CalculatedStyle ToDimension(this Bounds bounds) => new(bounds.X, bounds.Y, bounds.Width, bounds.Height);

    // public static VanillaMouseEvent ToVanillaMouseEvent(this SilkyMouseEvent evt, UIElement source) => new(source, evt.MousePosition);
}