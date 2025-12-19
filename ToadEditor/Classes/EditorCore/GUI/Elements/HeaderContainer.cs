using Prowl.PaperUI;
using Prowl.PaperUI.Events;
using Prowl.Vector;
using ToadEditor.Classes.Base;
using ToadEditor.Classes.EditorCore.GUI.Base;
using ToadEditor.Classes.EditorCore.Modules;

namespace ToadEditor.Classes.EditorCore.GUI.Elements;

public class HeaderContainer(DockSpaceManager dockManager)
{
    public void DrawHeader()
    {
        var dockContainer = dockManager.Dock(DockType.Top);
        using (dockManager.UI.Column("Header")
                   .PositionType(PositionType.SelfDirected)
                   .Size(dockContainer.Width, dockContainer.Height - 3f)
                   .Position(dockContainer.X, dockContainer.Y)
                   .BackgroundColor(Color32.FromArgb(255, 45, 45, 45))
                   .Enter())
        {
            HeaderBody();
        }
    }

    private void HeaderBody()
    {
        using (dockManager.UI.Row("HeaderContainer").Enter())
        {
            DrawEditorSettings();
            DrawEditorRuntimeSettings();
        }
    }

    private void DrawEditorSettings()
    {
        using (dockManager.UI.Row("EditorSettingsButtons").Width(dockManager.UI.Stretch(0.90f)).Enter())
        {
            Button("File", (e) =>
            {

            });

            Button("Windows", (e) =>
            {

            });

            Button("Project", (e) =>
            {

            });

            Button("Settings", (e) =>
            {

            });
        }
    }

    private void DrawEditorRuntimeSettings()
    {
        using (dockManager.UI.Row("EditorRuntimeButtons").Width(dockManager.UI.Stretch(1)).Enter())
        {
            Button(EditorRuntimeSettings.IsPlaying ? "Stop" : "Play", (e) =>
            {
                if (e.Button != PaperMouseBtn.Left) return;
                EditorStateManager.InvokePlayState();
            });
        }
    }

    private void Button(string name, Action<ClickEvent> interaction)
    {
        dockManager.UI.Box(name.Replace(" ", string.Empty))
            .BackgroundColor(Color32.FromArgb(255, 30, 30, 30))
            .Width(60)
            .Height(25)
            .Left(3f)
            .Top(5f)
            .Rounded(3f)
            .TextColor(Color.White)
            .Text(name, Fonts.Default)
            .Alignment(TextAlignment.MiddleCenter)
            .OnClick(interaction)
            .Hovered
            .BackgroundColor(Color.DarkGray)
            .End();
    }
}
