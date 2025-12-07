using Guinevere;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.GameObjects.Menus;

public class PauseMenu : Behavior
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

        using (UI.Node(UI.ScreenRect.Width, UI.ScreenRect.Height).Expand().Enter())
        {
            UI.DrawBackgroundRect(Color.FromArgb(128, 0, 0, 0));
            using (UI.Node().Expand().Margin(150).Gap(5f).AlignContent(0.5f).Direction(Axis.Vertical).Enter())
            {
                var backgroundColor = Color.FromArgb(155, 25, 25, 25);
                using (UI.Node(150, 30).Direction(Axis.Vertical).AlignContent(0.5f).Enter())
                {
                    var onHover = UI.CurrentNode.GetInteractable().OnHover();
                    UI.DrawBackgroundRect(backgroundColor, 12f);
                    UI.DrawText("Resume", 10f, onHover ? Color.DarkGray : Color.White).MarginBottom(5f);

                    if (UI.CurrentNode.GetInteractable().OnClick())
                    {
                        UpdatePausedState();
                        IsDrawingPauseMenu = false;
                    }
                }

                using (UI.Node(150, 30).Direction(Axis.Vertical).AlignContent(0.5f).Enter())
                {
                    var onHover = UI.CurrentNode.GetInteractable().OnHover();
                    UI.DrawBackgroundRect(backgroundColor, 12f);
                    UI.DrawText("Settings", 10f, onHover ? Color.DarkGray : Color.White).MarginBottom(5f);

                    if (UI.CurrentNode.GetInteractable().OnClick()) {}
                }

                using (UI.Node(150, 30).Direction(Axis.Vertical).AlignContent(0.5f).Enter())
                {
                    var onHover = UI.CurrentNode.GetInteractable().OnHover();
                    UI.DrawBackgroundRect(backgroundColor, 12f);
                    UI.DrawText("Exit", 10f, onHover ? Color.DarkGray : Color.White).MarginBottom(5f);

                    if (UI.CurrentNode.OnClick()) Environment.Exit(0);
                }
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
