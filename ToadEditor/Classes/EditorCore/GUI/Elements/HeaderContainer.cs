using Prowl.PaperUI;
using Prowl.Vector;
using ToadEditor.Classes.EditorCore.GUI.Base;

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

    public void HeaderBody()
    {

    }
}
