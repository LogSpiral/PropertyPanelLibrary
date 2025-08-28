using Microsoft.Xna.Framework;
using SilkyUIFramework;
using SilkyUIFramework.Animation;
using Terraria.Audio;
using Terraria.ID;

namespace PropertyPanelLibrary.BasicElements;

public class SUITriangleToggle : SUITriangleIcon
{
    private AnimationTimer _switchTimer = new();
    public Vector2[] OpenStateCoord { get; init; } = [new(0.0f, 0.3f), new(0.5f, 0.8f), new(1.0f, 0.3f)];
    public Vector2[] CloseStateCoord { get; init; } = [new(0.7f, 0.0f), new(0.2f, 0.5f), new(0.7f, 1.0f)];
    public bool State { get; set; }

    public SUITriangleToggle(bool initState = false)
    {
        if (initState)
            _switchTimer.ImmediateCompleted();
        UpdateTriangle();
    }

    public override void OnLeftMouseClick(UIMouseEvent evt)
    {
        State = !State;
        if (State) _switchTimer.StartUpdate();
        else _switchTimer.StartReverseUpdate();
        SoundEngine.PlaySound(SoundID.MenuTick);
        base.OnLeftMouseClick(evt);
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        _switchTimer.Update(gameTime);
        UpdateTriangle();
        base.UpdateStatus(gameTime);
    }

    private void UpdateTriangle()
    {
        for (int n = 0; n < 3; n++)
            trianglePercentCoord[n] = Vector2.Lerp(CloseStateCoord[n], OpenStateCoord[n], _switchTimer.Schedule);
    }
}