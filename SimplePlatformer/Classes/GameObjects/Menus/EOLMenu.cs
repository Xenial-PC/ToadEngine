using Prowl.PaperUI;
using Prowl.Vector;
using SimplePlatformer.Classes.GameObjects.Controllers;
using ToadEngine.Classes.Base.Scripting.Base;
using SavePointScript = SimplePlatformer.Classes.GameObjects.Scripts.SavePointScript;

namespace SimplePlatformer.Classes.GameObjects.Menus;

public class EOLMenu : MonoBehavior
{
    public static bool IsDrawingEOLMenu, IsDrawingLoseScreen;

    public void Start()
    {
        IsDrawingEOLMenu = false;
        IsDrawingLoseScreen = false;
    }

    public void OnGUI()
    {
        if (IsDrawingEOLMenu) WinScreen();
        if (IsDrawingLoseScreen) LoseScreen();
    }

    private void WinScreen()
    {
        using (UI.Box("WinScreen").Size(UI.ScreenRect.Size.X, UI.ScreenRect.Size.Y)
                   .BackgroundColor(Color32.FromArgb(128, 0, 0, 0)).Enter())
        {
            using (UI.Column("ElementContainer").Size(UI.Auto).Margin(UI.Stretch(1.0f)).Enter())
            {
                UI.Box("WinText")
                    .Text("You Win!", Fonts.Default)
                    .TextColor(Color.Green)
                    .Alignment(TextAlignment.MiddleCenter)
                    .Margin(UI.Stretch(1), UI.Stretch(1), 0, 20f);

                UI.Box("NextLevelButton")
                    .Width(100)
                    .Height(25)
                    .Text("Next Level", Fonts.Default)
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
                        PauseMenu.UpdatePausedState();
                        PlayerHud.Level++;

                        LoadScene($"Level{PlayerHud.Level}");
                        IsDrawingEOLMenu = false;
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

    private void LoseScreen()
    {
        using (UI.Box("LoseScreen").Size(UI.ScreenRect.Size.X, UI.ScreenRect.Size.Y)
                   .BackgroundColor(Color32.FromArgb(128, 0, 0, 0)).Enter())
        {
            using (UI.Column("ElementContainer").Size(UI.Auto).Margin(UI.Stretch(1.0f)).Enter())
            {
                UI.Box("LoseText")
                    .Text("You Have Fallen To Time!", Fonts.Default)
                    .TextColor(Color.DarkRed)
                    .Alignment(TextAlignment.MiddleCenter)
                    .Margin(UI.Stretch(1), UI.Stretch(1), 0, 20f);

                UI.Box("RetryLevelButton")
                    .Width(100)
                    .Height(25)
                    .Text("Restart Level", Fonts.Default)
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
                        PauseMenu.UpdatePausedState();

                        SavePointScript.SavePoint = new Vector3(0f, 14f, 0f);
                        LoadScene($"Level{PlayerHud.Level}");
                        IsDrawingLoseScreen = false;
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
}
