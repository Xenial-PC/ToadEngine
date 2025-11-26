using Guinevere;
using SimplePlatformer.Classes.GameObjects.Menus;
using SkiaSharp;
using ToadEngine.Classes.Base.UI;

namespace SimplePlatformer.Classes.GameObjects.Scripts;

public class PlayerHud : Behaviour
{
    public static int Level = 1;
    private static float _doubleJumpStaminaSlider;

    public override void OnGUI()
    {
        base.OnGUI();
        if (PauseMenu.IsPaused) return;

        /*UI.DrawRect(new Rect(15, UI.ScreenRect.Height - 40, 200, 20), Color.Gray, 12f).Expand(0.25f);
        UI.DrawRect(new Rect(15, UI.ScreenRect.Height - 40, _doubleJumpStaminaSlider * 200, 20), Color.Purple, 12f).Expand(0.25f);*/

        using (UI.Node(UI.ScreenRect.Width, UI.ScreenRect.Height).Enter())
        {
            UI.DrawRect(new Rect(20, 20, 65, 35), Color.FromArgb(128, 0, 0, 0), 12f);
            UI.DrawText($"Level: {Level}", 28.5f, 42.5f, 15f, Color.Purple);

            var playerStaminaWidth = UI.CurrentNode.Rect.W * 0.30f;
            UI.DrawRect(new Rect(15, UI.ScreenRect.Height - 40, playerStaminaWidth, 20), Color.Gray, 12f);
            UI.DrawRect(new Rect(15, UI.ScreenRect.Height - 40, _doubleJumpStaminaSlider * playerStaminaWidth, 20), Color.Purple, 12f);
        }

        UI.DrawCircleFilled(new Vector2((UI.ScreenRect.Width / 2f), (UI.ScreenRect.Height / 2f)), 3f, Color.GhostWhite);
    }

    public static void UpdateStaminaUI(float value)
    {
        _doubleJumpStaminaSlider = value switch
        {
            < 0 => 0,
            > 1 => 1,
            _ => value
        };
    }
}
