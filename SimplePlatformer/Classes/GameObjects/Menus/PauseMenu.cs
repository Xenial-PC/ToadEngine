using Prowl.PaperUI;
using Prowl.Vector;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.GameObjects.Menus;

public class PauseMenu : MonoBehavior
{
    public static bool IsPaused, IsDrawingPauseMenu;
    private static Vector2 _mousePositionCache;

    public void Start()
    {
        IsPaused = false;
        IsDrawingPauseMenu = false;
    }

    public void OnGUI()
    {
        if ((!IsPaused && !IsDrawingPauseMenu) || EOLMenu.IsDrawingEOLMenu || EOLMenu.IsDrawingLoseScreen) return;

        using (UI.Box("PauseScreen").Size(UI.ScreenRect.Size.X, UI.ScreenRect.Size.Y)
                   .BackgroundColor(Color32.FromArgb(128, 0, 0, 0)).Enter())
        {
            using (UI.Column("ElementContainer").Size(UI.Auto).Margin(UI.Stretch(1.0f)).Enter())
            {
                UI.Box("HeaderText")
                    .Text("Simple Platformer!", Fonts.Default)
                    .TextColor(Color.Aqua)
                    .Alignment(TextAlignment.MiddleCenter)
                    .Margin(UI.Stretch(1), UI.Stretch(1), 0, 20f);

                UI.Box("ResumeButton")
                    .Width(100)
                    .Height(25)
                    .Text("Resume", Fonts.Default)
                    .TextColor(Color.White)
                    .Alignment(TextAlignment.MiddleCenter)
                    .BackgroundColor(Color.Black)
                    .Rounded(5f)
                    .Margin(0, 0, 0, 5f)
                    .Hovered
                        .BackgroundColor(Color.DarkGray)
                    .End()
                    .OnClick((value) =>
                    {
                        UpdatePausedState();
                        IsDrawingPauseMenu = false;
                    });

                UI.Box("ExitButton")
                    .Width(100)
                    .Height(25)
                    .Text("Exit", Fonts.Default)
                    .TextColor(Color.White)
                    .Alignment(TextAlignment.MiddleCenter)
                    .BackgroundColor(Color.Black)
                    .Rounded(5f)
                    .Hovered
                        .BackgroundColor(Color.DarkGray)
                    .End()
                    .OnClick((value) =>
                    {
                        Environment.Exit(0);
                    });
            }
        }
    }

    public void Update()
    {
        if (Input.IsKeyPressed(Keys.Escape))
        {
            IsDrawingPauseMenu = !IsDrawingPauseMenu;
            Physics.IsPhysicsPaused = !Physics.IsPhysicsPaused;
            UpdatePausedState();
        }
    }

    public static void UpdatePausedState()
    {
        IsPaused = !IsPaused;

        var nativeWindow = Service.NativeWindow;
        nativeWindow.CursorState = IsPaused ? CursorState.Normal : CursorState.Grabbed;
        nativeWindow.MousePosition = _mousePositionCache;

        if (IsPaused) _mousePositionCache = nativeWindow.MousePosition;
    }
}
