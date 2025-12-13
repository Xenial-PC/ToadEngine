using Guinevere;
using ToadEditor.Classes.EditorCore.GUI.Base;
using ToadEditor.Classes.EditorCore.GUI.Components;

namespace ToadEditor.Classes.EditorCore.GUI.Elements;

public class ConsoleTab(DockSpaceManager.Docks dock) : TabMenu(dock)
{
    public override void TabBody(LayoutNode node)
    {
        UI.DrawRect(node.Rect, Color.FromArgb(255, 3, 3, 3));
    }
}
