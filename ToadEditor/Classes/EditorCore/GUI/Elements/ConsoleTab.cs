using Guinevere;
using ToadEditor.Classes.EditorCore.GUI.Base;
using ToadEditor.Classes.EditorCore.GUI.Components;

namespace ToadEditor.Classes.EditorCore.GUI.Elements;

public class ConsoleTab(DockSpaceManager.Docks dock) : TabMenu(dock)
{
    public override void TabBody(Rect windowPos)
    {
        UI.DrawRect(windowPos, Color.FromArgb(255, 20, 20, 20));
    }
}
