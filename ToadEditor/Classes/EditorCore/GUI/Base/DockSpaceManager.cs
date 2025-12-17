using Prowl.PaperUI;
using Prowl.Scribe;
using Prowl.Vector;

namespace ToadEditor.Classes.EditorCore.GUI.Base;

public enum DockType
{
    None,
    Top,
    Left,
    Middle,
    Right,
    Bottom,
}

public class DockSpaceManager()
{
    public Paper UI => ToadEngine.Classes.Base.UI.GUI.UI;
    private readonly List<DockSpace> _dockSpaces = new();

    public RectangleF Dock(DockType dockType) => GetDockSpace(dockType)!.DockSize;

    public void InitDockSpaces()
    {
        CreateDefaultDocks();
    }

    private void CreateDefaultDocks()
    {
        CreateDock(DockType.Top,
            () => new RectangleF(0, 0, UI.ScreenRect.Size.X, 40));

        CreateDock(DockType.Left, 
            () =>
            {
                var topDock = GetDockSpace(DockType.Top);
                return topDock == null ? default : new RectangleF(0, topDock.DockSize.Height, UI.ScreenRect.Size.X * 0.20f, (UI.ScreenRect.Size.Y - topDock.DockSize.Height) - 200f);
            });

        CreateDock(DockType.Middle,
            () =>
            {
                var leftDock = GetDockSpace(DockType.Left);
                if (leftDock == null) return default;

                return new RectangleF(leftDock.DockSize.Width, leftDock.DockSize.Y, UI.ScreenRect.Size.X - (leftDock.DockSize.Width * 2f),
                    leftDock.DockSize.Height);
            });

        CreateDock(DockType.Right,
            () =>
            {
                var topDock = GetDockSpace(DockType.Top);
                var middleDock = GetDockSpace(DockType.Middle);
                var leftDock = GetDockSpace(DockType.Left);
                if (topDock == null || middleDock == null || leftDock == null) return default;

                return new RectangleF(middleDock.DockSize.X + middleDock.DockSize.Width, leftDock.DockSize.Y,
                    (UI.ScreenRect.Size.X - (leftDock.DockSize.Width - middleDock.DockSize.Width)), (UI.ScreenRect.Size.Y - topDock.DockSize.Height));
            });

        CreateDock(DockType.Bottom,
            () =>
            {
                var topDock = GetDockSpace(DockType.Top);
                var middleDock = GetDockSpace(DockType.Middle);
                var rightDock = GetDockSpace(DockType.Right);
                var leftDock = GetDockSpace(DockType.Left);
                if (middleDock == null || rightDock == null || leftDock == null || topDock == null) return default;

                return new RectangleF(0, middleDock.DockSize.Y + middleDock.DockSize.Height,
                    middleDock.DockSize.Width + leftDock.DockSize.Width, UI.ScreenRect.Size.Y - (topDock.DockSize.Height - middleDock.DockSize.Height));
            });
    }

    private void CreateDock(DockType dockType, DockSpace.ResizeHandler size)
    {
        var dockSpace = new DockSpace()
        {
            DockType = dockType,
            ResizeCallback = size
        };

        _dockSpaces.Add(dockSpace);
    }

    private DockSpace? GetDockSpace(DockType dockType) => _dockSpaces.FirstOrDefault(dockSpace => dockSpace.DockType == dockType);
}

public class DockSpace
{
    public delegate RectangleF ResizeHandler();
    public DockType DockType;

    public RectangleF DockSize => ResizeCallback.Invoke();
    public ResizeHandler ResizeCallback = null!;
}
