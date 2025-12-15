using System.Timers;
using Assimp;
using Prowl.PaperUI;
using Prowl.PaperUI.LayoutEngine;
using Prowl.Vector;
using SimplePlatformer.Classes.GameObjects.Menus;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Textures;
using Timer = System.Timers.Timer;

namespace SimplePlatformer.Classes.GameObjects.Controllers;

public class PlayerHud : Behavior
{
    public static int Level = 1;
    private float _doubleJumpSlider, _boostSlider, _healthSlider;

    public static Timer LevelTimer = null!;
    public TimeSpan Time;
    private static DateTime _levelStartTime;
    private static bool _isTimerRunning;

    public void Start()
    {
        LevelTimer = new Timer(1);
        LevelTimer.Elapsed += LevelTimerOnElapsed;
        LevelTimer.AutoReset = true;
    }

    private void LevelTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        Time = DateTime.Now - _levelStartTime;
    }

    public void OnGUI()
    {
        if (PauseMenu.IsPaused) return;
        using (UI.Box("PlayerHud").Size(UI.ScreenRect.Size.X, UI.ScreenRect.Size.Y).Enter())
        {
            UI.Canvas.RoundedRectFilled(20, 20, 65, 35, 12f, Color32.FromArgb(128, 0, 0, 0));
            UI.Box("LevelText")
                .PositionType(PositionType.SelfDirected)
                .BackgroundColor(Color32.FromArgb(128, 0, 0, 0))
                .Size(65, 35)
                .Left(20)
                .Top(20)
                .Rounded(12f)
                .Text($"Level {Level}", GUI.Fonts.Default)
                .BackgroundColor(Color.Transparent)
                .Alignment(TextAlignment.MiddleCenter)
                .TextColor(Color.Purple);

            using (UI.Row("Timer")
                       .PositionType(PositionType.SelfDirected)
                       .BackgroundColor(Color32.FromArgb(128, 0, 0, 0))
                       .Size(105, 50)
                       .Left(UI.ScreenRect.Size.X - 120)
                       .Top(20)
                       .Rounded(12f).Enter())
            {
                UI.Box("TimerImage")
                    .PositionType(PositionType.SelfDirected)
                    .Image(Texture.FromPath($"Resources/Textures/UI/timer.png", TextureType.Diffuse))
                    .Height(50)
                    .Width(50);

                UI.Box("TimeText")
                    .Text($"{Time.Minutes}:{Time.Seconds}:{Time.Milliseconds}", GUI.Fonts.Default)
                    .Alignment(TextAlignment.MiddleRight)
                    .PositionType(PositionType.SelfDirected)
                    .TextColor(Color.Purple)
                    .Margin(0, 10, 0, 0);
            }

            using (UI.Column("Information").PositionType(PositionType.SelfDirected)
                       .Size(450, 80)
                       .Top(UI.ScreenRect.Size.Y - 80).Enter())
            {
                ValueSlider("HealthSlider",
                    information: (value: _healthSlider, width: 400, height: 20),
                    position: (top: 5f, left: 20),
                    colors: (backgroundColor: Color.Gray, foreGroundColor: Color.DarkRed));

                ValueSlider("BoostSlider",
                    information: (value: _boostSlider, width: 400, height: 20),
                    position: (top: 5f, left: 20),
                    colors: (backgroundColor: Color.Gray, foreGroundColor: Color.Teal));

                ValueSlider("StaminaSlider",
                    information: (value: _doubleJumpSlider, width: 400, height: 20),
                    position: (top: 5f, left: 20),
                    colors: (backgroundColor: Color.Gray, foreGroundColor: Color.Purple));
            }
        }

        UI.Canvas.CircleFilled(UI.ScreenRect.Center.X, UI.ScreenRect.Center.Y, 3f, Color.GhostWhite);
    }

    private void ValueSlider(string sliderName, 
        (float value, float width, float height) information, 
        (UnitValue top, UnitValue left) position, 
        (Color32 backgroundColor, Color32 foreGroundColor) colors)
    {
        using (UI.Box($"{sliderName}Background")
                   .BackgroundColor(colors.backgroundColor)
                   .Rounded(12f)
                   .Top(position.top)
                   .Left(position.left)
                   .Height(information.height)
                   .Width(information.width).Enter())
        {
            UI.Box($"{sliderName}")
                .PositionType(PositionType.SelfDirected)
                .Height(information.height)
                .Width(information.value * information.width)
                .BackgroundColor(colors.foreGroundColor)
                .Rounded(12f);
        }
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

    public void UpdateStaminaUI(float value) => MapSlider(out _doubleJumpSlider, value);
    public void UpdateBoostUI(float value) => MapSlider(out _boostSlider, value);
    public void UpdateHealthUI(float value) => MapSlider(out _healthSlider, value);
    
    private void MapSlider(out float slider, float value)
        => slider = value switch
        {
            < 0 => 0,
            > 1 => 1,
            _ => value
        };
}
