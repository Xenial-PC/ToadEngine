using Guinevere;
using SimplePlatformer.Classes.GameObjects.Scripts;

namespace SimplePlatformer.Classes.GameObjects.Menus;

public class EOLMenu : Behaviour
{
    public static bool IsDrawingEOLMenu;

    public override void Setup()
    {
        base.Setup();
        IsDrawingEOLMenu = false;
    }

    public override void OnGUI()
    {
        base.OnGUI();

        if (!IsDrawingEOLMenu) return;

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

                        GetWindow().LoadScene($"Level{PlayerHud.Level}");
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

    public override void Dispose()
    {
        base.Dispose();
    }
}
