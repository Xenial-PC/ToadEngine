using Prowl.PaperUI;
using Prowl.Scribe;
using Prowl.Vector;
using ToadEditor.Classes.EditorCore.GUI.Base;
using TextAlignment = Prowl.PaperUI.TextAlignment;

namespace ToadEditor.Classes.EditorCore.GUI.Components;

public class TabMenu
{
    private static readonly Dictionary<DockType, List<TabMenu>> _tabMenus = new();

    public Paper UI => ToadEngine.Classes.Base.UI.GUI.UI;

    public bool IsSelected;
    public string TabName;

    public DockType DockedAt;

    public TabMenu(DockType dockType, string tabName)
    {
        DockedAt = dockType;
        TabName = tabName;

        if (_tabMenus.TryGetValue(dockType, out var list))
        {
            list.Add(this);
            return;
        }

        _tabMenus.Add(dockType, [this]);
    }

    public static void DrawTabs(DockSpaceManager dock)
    {
        if (_tabMenus.TryGetValue(DockType.Left, out var left))
            SetupTabContainer(dock, DockType.Left, left);

        if (_tabMenus.TryGetValue(DockType.Middle, out var middle))
            SetupTabContainer(dock, DockType.Middle, middle);

        if (_tabMenus.TryGetValue(DockType.Right, out var right))
            SetupTabContainer(dock, DockType.Right, right);

        if (_tabMenus.TryGetValue(DockType.Bottom, out var bottom))
            SetupTabContainer(dock, DockType.Bottom, bottom);
    }

    private static void SetupTabContainer(DockSpaceManager dock, DockType dockType, List<TabMenu> tabMenu)
    {
        var dockContainer = dock.Dock(dockType);
        using (dock.UI.Column(dockType.ToString())
                   .PositionType(PositionType.SelfDirected)
                   .Size(dockContainer.Width - 3f, dockContainer.Height -3f)
                   .Position(dockContainer.X, dockContainer.Y)
                   .Enter())
        {
            using (dock.UI.Row($"{dockType.ToString()}_Tabs")
                       .BackgroundColor(Color32.FromArgb(255, 35, 35, 35))
                       .Rounded(2f, 2f, 0, 0)
                       .SetScroll(Scroll.ScrollX).Enter())
            {
                foreach (var tab in tabMenu) 
                    tab.DrawTabHeader();
            }

            var selectedTab = tabMenu.FirstOrDefault(selected => selected.IsSelected);
            if (selectedTab == null) return;
            selectedTab.DrawTab(dockContainer);
        }
    }

    private void DrawTabHeader()
    {
        UI.Box($"{TabName}_Header").Size(95, 20)
            .Text($"{TabName}", Fonts.Default)
            .TextColor(Color.White)
            .Alignment(TextAlignment.MiddleCenter)
            .Margin(0, 5f, 0, 0)
            .BackgroundColor(Color32.FromArgb(255, 25, 25 , 25))
            .Rounded(2f, 2f, 0f, 0f)
            .Hovered
            .BackgroundColor(Color32.FromArgb(255, 55, 55, 55))
            .End()
            .OnClick((e) =>
            {
                if (e.Button != PaperMouseBtn.Left) return;
                DisableTabs();
                IsSelected = true;
            })
            .If(IsSelected)
            .BackgroundColor(Color32.FromArgb(255, 55, 55, 55))
            .End();
    }

    private void DisableTabs()
    {
        foreach (var tab in _tabMenus
                     .Where(tab => tab.Key == DockedAt)
                     .Select(tab => tab.Value)
                     .First().Where(tab => tab != this)) tab.IsSelected = false;
    }

    private void DrawTab(RectangleF containerSize)
    {
        using (UI.Box($"{TabName}_Body")
                   .BackgroundColor(Color32.FromArgb(255, 45, 45, 45))
                   .Size(containerSize.Width - 3f, containerSize.Height - 22f)
                   .BorderColor(Color.Transparent)
                   .Enter())
        {
            TabBody(containerSize);
        }
    }

    public virtual void TabBody(RectangleF containerSize)
    {
        
    }
}
