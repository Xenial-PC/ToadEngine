using Prowl.Paper.Utilities;
using Prowl.PaperUI;
using Prowl.Vector;
using ToadEditor.Classes.EditorCore.GUI.Base;

namespace ToadEditor.Classes.EditorCore.GUI.Components;

public class TabMenu
{
    private static readonly Dictionary<DockSpaceManager.Docks, List<TabMenu>> _tabMenus = new();

    public Paper UI => ToadEngine.Classes.Base.UI.GUI.UI;

    public bool IsSelected;
    public string TabName = string.Empty;

    public float HeaderWidth = 95f;

    public DockSpaceManager.Docks Docked;

    public TabMenu(DockSpaceManager.Docks dock)
    {
        Docked = dock;

        if (_tabMenus.TryGetValue(dock, out var list))
        {
            list.Add(this);
            return;
        }

        _tabMenus.Add(dock, [this]);
    }

    public static void DrawTabs(DockSpaceManager dock)
    {
        float headerOffsetLeft = 0, headerOffsetRight = 0, headerOffsetMiddle = 0, headerOffsetBottom = 0;
        foreach (var tabMenu in _tabMenus)
        {
            switch (tabMenu.Key)
            {
                case DockSpaceManager.Docks.Left:
                    foreach (var tab in tabMenu.Value)
                        SetupTab(dock, tab, ref headerOffsetLeft);
                    break;
                case DockSpaceManager.Docks.Middle:
                    foreach (var tab in tabMenu.Value)
                        SetupTab(dock, tab, ref headerOffsetMiddle);
                    break;
                case DockSpaceManager.Docks.Right:
                    foreach (var tab in tabMenu.Value)
                        SetupTab(dock, tab, ref headerOffsetRight);
                    break;
                case DockSpaceManager.Docks.Bottom:
                    foreach (var tab in tabMenu.Value)
                        SetupTab(dock, tab, ref headerOffsetBottom);
                    break;
            }
        }
    }

    private static void SetupTab(DockSpaceManager dock, TabMenu tabMenu, ref float headerOffsetLeft)
    {
        tabMenu.DrawTab(dock, headerOffsetLeft);
        headerOffsetLeft += tabMenu.HeaderWidth;
    }

    private void DrawTab(DockSpaceManager dock, float headerOffset)
    {
        var dockPos = dock.Dock(Docked);
        using (UI.Node(dockPos.Width, dockPos.Height).Left(dockPos.X).Top(dockPos.Y).Enter())
        {
            var currentNode = UI.CurrentNode;
            var windowPos = currentNode.Rect;
            using (currentNode.Enter())
            {
                using (UI.Node(HeaderWidth, 25).Left(windowPos.X + headerOffset).Top(windowPos.Y - 25 + 5f).Enter())
                {
                    var headerNode = UI.CurrentNode;
                    
                    var isClicked = headerNode.GetInteractable().OnClick(Guinevere.MouseButton.Left);
                    if (isClicked) IsSelected = true;

                    UI.DrawRect(headerNode.Rect, IsSelected ? ColorUtil.FromArgb(255, 5, 5, 5) : Color.Black, 3);

                    using (UI.Node(headerNode.Rect.W, headerNode.Rect.H).Top(headerNode.Rect.Y).Left(headerNode.Rect.X).Enter())
                    {
                        var headerTextNode = UI.CurrentNode;
                        
                        var xPos = headerTextNode.Rect.X + 10f;
                        var yPos = headerTextNode.Center.Y + 5f;
                        UI.DrawText(TabName, xPos, yPos, 15, Color.White);
                    }
                }

                if (!IsSelected) return;
                TabBody(currentNode);
            }
        }
    }

    public virtual void TabBody()
    {

    }
}
