using Prowl.Paper.Utilities;
using Prowl.PaperUI;
using Prowl.Vector;

namespace ToadEditor.Classes.EditorCore.GUI.Base;

public class DockSpaceManager()
{
    private Paper UI => ToadEngine.Classes.Base.UI.GUI.UI;

    private readonly List<DockSpace> _dockSpaces = new();

    private Action? DrawDockSpace;

    public enum Docks
    {
        None,
        Header,
        Top,
        Left,
        Middle,
        Right,
        Bottom,
    }

    public void InitDockSpaces()
    {
        CreateDock(Docks.Header, ColorUtil.FromArgb(255, 10, 10, 10));
        CreateDock(Docks.Top, ColorUtil.FromArgb(255, 3, 3, 3));
        CreateDock(Docks.Left, Color.Transparent);
        CreateDock(Docks.Middle, Color.Transparent);
        CreateDock(Docks.Right, Color.Transparent);
        CreateDock(Docks.Bottom, Color.Transparent);
    }

    public Rect Dock(Docks dock) => GetDockSpace(dock).DockSize;

    public void DrawDockSpaces()
    {
        UpdateDockSpaces();
        DrawDockSpace?.Invoke();
    }

    private void CreateDock(Docks dockPosition, Color dockColor)
    {
        var dockSpace = new DockSpace()
        {
            Dock = dockPosition,
            DockColor = dockColor
        };
        _dockSpaces.Add(dockSpace);

        DrawDockSpace += () =>
        {
            var dock = GetDockSpace(dockPosition);
            using (UI.Node(dock.DockSize.Width, dock.DockSize.Height).Enter())
            {
                var dockNode = UI.CurrentNode;
                dockNode.Rect = dock.DockSize;

                UI.DrawRect(dockNode.Rect, dock.DockColor, 5);
            }
        };
    }

    private void UpdateDockSpaces()
    {
        var screenHeightPercentage = 0.045f;
        var dockRect = new Rect(0, 0, UI.ScreenRect.Width, UI.ScreenRect.Height * screenHeightPercentage);

        var headerDock = GetDockSpace(Docks.Header);
        headerDock.DockSize = dockRect;

        var topDock = GetDockSpace(Docks.Top);
        screenHeightPercentage = 0.025f;
        dockRect = new Rect(0, headerDock.DockSize.Height, headerDock.DockSize.Width,
            UI.ScreenRect.Height * screenHeightPercentage);

        topDock.DockSize = dockRect;

        var leftDock = GetDockSpace(Docks.Left);
        var dockHeight = topDock.DockSize.Height + headerDock.DockSize.Height;
        var finalDockHeight = UI.ScreenRect.Height - dockHeight;
        var screenWidthPercentage = 0.2f;
        screenHeightPercentage = 0.75f;
        var yPos = topDock.DockSize.Y + topDock.DockSize.Height;

        dockRect = new Rect(0, yPos, UI.ScreenRect.W * screenWidthPercentage, finalDockHeight * screenHeightPercentage);
        leftDock.DockSize = dockRect;

        var middleDock = GetDockSpace(Docks.Middle);
        var dockWidth = UI.ScreenRect.Width - leftDock.DockSize.Width * 2.5f;
        dockRect = new Rect(leftDock.DockSize.Width, leftDock.DockSize.Y, dockWidth, finalDockHeight * screenHeightPercentage);
        middleDock.DockSize = dockRect;

        var bottomDock = GetDockSpace(Docks.Bottom);
        yPos += middleDock.DockSize.Height;
        dockHeight += middleDock.DockSize.Height;
        finalDockHeight = UI.ScreenRect.Height - dockHeight;
        dockWidth = leftDock.DockSize.Width + middleDock.DockSize.Width;

        dockRect = new Rect(0, yPos, dockWidth, finalDockHeight);
        bottomDock.DockSize = dockRect;

        var rightDock = GetDockSpace(Docks.Right);
        dockWidth = UI.ScreenRect.Width - (leftDock.DockSize.Width + middleDock.DockSize.Width);
        finalDockHeight = leftDock.DockSize.Height + bottomDock.DockSize.Height;
        yPos = topDock.DockSize.Y + topDock.DockSize.Height;

        dockRect = new Rect(middleDock.DockSize.W + leftDock.DockSize.W, yPos, dockWidth, finalDockHeight);
        rightDock.DockSize = dockRect;
    }

    private DockSpace GetDockSpace(Docks dock) => _dockSpaces.FirstOrDefault(dockSpace => dockSpace.Dock == dock)!;
}

public class DockSpace
{
    public DockSpaceManager.Docks Dock;
    public Rect DockSize = new();

    public Color DockColor;
}
