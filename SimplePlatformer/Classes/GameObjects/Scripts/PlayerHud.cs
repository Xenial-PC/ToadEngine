using System.Timers;
using Guinevere;
using SimplePlatformer.Classes.GameObjects.Menus;
using SkiaSharp;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Base.UI;
using Timer = System.Timers.Timer;

namespace SimplePlatformer.Classes.GameObjects.Scripts;

public class PlayerHud : Behavior
{
    public static int Level = 1;
    private float _doubleJumpSlider, _boostSlider, _healthSlider;

    public static Timer LevelTimer = null!;
    public TimeSpan Time;
    private static DateTime _levelStartTime;
    private static bool _isTimerRunning;

    public override void Setup()
    {
        LevelTimer = new Timer(1);
        LevelTimer.Elapsed += LevelTimerOnElapsed;
        LevelTimer.AutoReset = true;
    }

    private void LevelTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        Time = DateTime.Now - _levelStartTime;
    }

    public override void OnGUI()
    {
        if (PauseMenu.IsPaused) return;

        using (UI.Node(UI.ScreenRect.Width, UI.ScreenRect.Height).Enter())
        {
            UI.DrawRect(new Rect(20, 20, 65, 35), Color.FromArgb(128, 0, 0, 0), 12f);
            UI.DrawText($"Level: {Level}", 28.5f, 42.5f, 15f, Color.Purple);

            UI.DrawRect(new Rect(UI.CurrentNode.Rect.W - 120, 20, 105, 35), Color.FromArgb(128, 0, 0, 0), 12f);
            UI.DrawText($"{Time.Minutes}:{Time.Seconds}:{Time.Milliseconds}", UI.CurrentNode.Rect.W - 80, 42.5f, 15f, Color.Purple);
            UI.DrawImage($"Resources/Textures/UI/timer.png", UI.CurrentNode.Rect.W - 120, 20, 35, 35);

            var width = UI.CurrentNode.Rect.W * 0.30f;
            UI.DrawRect(new Rect(15, UI.ScreenRect.Height - 40, width, 20), Color.Gray, 12f);
            UI.DrawRect(new Rect(15, UI.ScreenRect.Height - 40, _doubleJumpSlider * width, 20), Color.Purple, 12f);
            UI.DrawImage($"Resources/Textures/UI/jumpStamina.png", 20, UI.ScreenRect.Height - 42, 25, 25);

            UI.DrawRect(new Rect(15, UI.ScreenRect.Height - 65, width, 20), Color.Gray, 12f);
            UI.DrawRect(new Rect(15, UI.ScreenRect.Height - 65, _boostSlider * width, 20), Color.Yellow, 12f);
            UI.DrawImage($"Resources/Textures/UI/boost.png", 15, UI.ScreenRect.Height - 85, 45, 45, Color.Black);

            UI.DrawRect(new Rect(15, UI.ScreenRect.Height - 90, width, 20), Color.Gray, 12f);
            UI.DrawRect(new Rect(15, UI.ScreenRect.Height - 90, _healthSlider * width, 20), Color.DarkRed, 12f);
            //UI.DrawImage($"Resources/Textures/UI/health.png", 15, UI.ScreenRect.Height - 93, 35, 30);
        }

        UI.DrawCircleFilled(new Vector2((UI.ScreenRect.Width / 2f), (UI.ScreenRect.Height / 2f)), 3f, Color.GhostWhite);
    }

    public static void StartTimer()
    {
        if (_isTimerRunning) return;
        _isTimerRunning = true;

        _levelStartTime = DateTime.Now;
        LevelTimer.Enabled = _isTimerRunning;
    }

    public static void StopTimer()
    {
        if (!_isTimerRunning) return;
        _isTimerRunning = false;

        LevelTimer.Enabled = _isTimerRunning;
    }

    public void UpdateStaminaUI(float value)
    {
        _doubleJumpSlider = value switch
        {
            < 0 => 0,
            > 1 => 1,
            _ => value
        };
    }

    public void UpdateBoostUI(float value)
    {
        _boostSlider = value switch
        {
            < 0 => 0,
            > 1 => 1,
            _ => value
        };
    }

    public void UpdateHealthUI(float value)
    {
        _healthSlider = value switch
        {
            < 0 => 0,
            > 1 => 1,
            _ => value
        };
    }
}
