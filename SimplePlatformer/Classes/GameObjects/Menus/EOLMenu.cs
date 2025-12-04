using Guinevere;
using SimplePlatformer.Classes.GameObjects.Event;
using SimplePlatformer.Classes.GameObjects.Scripts;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.GameObjects.Menus;

public class EOLMenu : Behavior
{
    public static bool IsDrawingEOLMenu, IsDrawingLoseScreen;

    public override void OnStart()
    {
        IsDrawingEOLMenu = false;
        IsDrawingLoseScreen = false;
    }

    public override void OnGUI()
    {
        if (IsDrawingEOLMenu) WinScreen();
        if (IsDrawingLoseScreen) LoseScreen();
    }

    private void WinScreen()
    {
        using (UI.Node(UI.ScreenRect.Width, UI.ScreenRect.Height).Expand().Enter())
        {
            UI.DrawBackgroundRect(Color.FromArgb(128, 0, 0, 0));
            using (UI.Node().Expand().Margin(150).Gap(5f).AlignContent(0.5f).Direction(Axis.Vertical).Enter())
            {
                var backgroundColor = Color.FromArgb(155, 25, 25, 25);
                using (UI.Node(150, 30).Direction(Axis.Vertical).AlignContent(0f).MarginBottom(15f).Enter())
                {
                    UI.DrawText("Congrats You Win!!", 20f, Color.SpringGreen);
                }

                using (UI.Node(150, 30).Direction(Axis.Vertical).AlignContent(0.5f).Enter())
                {
                    var onHover = UI.CurrentNode.GetInteractable().OnHover();
                    UI.DrawBackgroundRect(backgroundColor, 12f);
                    UI.DrawText("Next Level", 10f, onHover ? Color.DarkGray : Color.White).MarginBottom(5f);

                    if (UI.CurrentNode.GetInteractable().OnClick())
                    {
                        PauseMenu.UpdatePausedState();
                        PlayerHud.Level++;

                        LoadScene($"Level{PlayerHud.Level}");
                        IsDrawingEOLMenu = false;
                    }
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

    private void LoseScreen()
    {
        using (UI.Node(UI.ScreenRect.Width, UI.ScreenRect.Height).Expand().Enter())
        {
            UI.DrawBackgroundRect(Color.FromArgb(128, 0, 0, 0));
            using (UI.Node().Expand().Margin(150).Gap(5f).AlignContent(0.5f).Direction(Axis.Vertical).Enter())
            {
                var backgroundColor = Color.FromArgb(155, 25, 25, 25);
                using (UI.Node(150, 30).Direction(Axis.Vertical).AlignContent(0f).MarginBottom(15f).MarginRight(80f).Enter())
                {
                    UI.DrawText("You Have Fallen To Time!", 20f, Color.DarkRed);
                }

                using (UI.Node(150, 30).Direction(Axis.Vertical).AlignContent(0.5f).Enter())
                {
                    var onHover = UI.CurrentNode.GetInteractable().OnHover();
                    UI.DrawBackgroundRect(backgroundColor, 12f);
                    UI.DrawText("Restart Level", 10f, onHover ? Color.DarkGray : Color.White).MarginBottom(5f);

                    if (UI.CurrentNode.GetInteractable().OnClick())
                    {
                        PauseMenu.UpdatePausedState();

                        SavePointScript.SavePoint = new Vector3(0f, 14f, 0f);
                        LoadScene($"Level{PlayerHud.Level}");
                        IsDrawingLoseScreen = false;
                    }
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
}
