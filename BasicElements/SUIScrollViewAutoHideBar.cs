using Microsoft.Xna.Framework;
using SilkyUIFramework;
using SilkyUIFramework.Animation;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;

namespace PropertyPanelLibrary.BasicElements;

[XmlElementMapping("ScrollViewAutoHideBar")]
public class SUIScrollViewAutoHideBar : SUIScrollView
{
    private AnimationTimer _scrollBarTimer = new();
    private bool _initNeeded = true;

    protected override void UpdateStatus(GameTime gameTime)
    {
        bool flag =
            Direction == Direction.Horizontal
            ? Mask.OuterBounds.Width >= Container.OuterBounds.Width - 2
            : Mask.OuterBounds.Height >= Container.OuterBounds.Height - 2;
        if (flag)
        {
            if (_initNeeded)
            {
                _initNeeded = false;
                _scrollBarTimer.ImmediateReverseCompleted();
            }
            else if (_scrollBarTimer.IsForward)
                _scrollBarTimer.StartReverseUpdate();
        }
        else
        {
            if (_initNeeded)
            {
                _initNeeded = false;
                _scrollBarTimer.ImmediateCompleted();
            }
            else if (_scrollBarTimer.IsReverse)
                _scrollBarTimer.StartUpdate();
        }
        _scrollBarTimer.Update(gameTime);

        float factor = _scrollBarTimer.Schedule;
        if (Direction == Direction.Horizontal)
            ScrollBar.SetHeight(4 * factor);
        else
            ScrollBar.SetWidth(4 * factor);
        Gap = new Size(8 * factor);
        base.UpdateStatus(gameTime);
    }
}