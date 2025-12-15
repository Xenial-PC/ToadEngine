using Prowl.Paper.Utilities;
using ToadEditor.Classes.EditorCore.GUI.Base;
using ToadEditor.Classes.EditorCore.GUI.Components;

namespace ToadEditor.Classes.EditorCore.GUI.Elements;

public class ConsoleTab(DockSpaceManager.Docks dock) : TabMenu(dock)
{
    public override void TabBody()
    {
        //UI.DrawRect(node.Rect, ColorUtil.FromArgb(255, 3, 3, 3));
    }
}
