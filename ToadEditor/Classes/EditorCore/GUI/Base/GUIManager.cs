using ToadEditor.Classes.EditorCore.GUI.Components;
using ToadEditor.Classes.EditorCore.GUI.Elements;
using ToadEditor.Classes.EditorCore.Renderer;

namespace ToadEditor.Classes.EditorCore.GUI.Base;

public class GUIManager(EditorRenderTarget target)
{
    public DockSpaceManager DockSpaceManager = new();

    public void Setup()
    {
        SetupDefaults();
        ToadEngine.Classes.Base.UI.GUI.GuiCallBack += EditorCallback;
    }

    private void SetupDefaults()
    {
        DockSpaceManager.InitDockSpaces();
        SetupDefaultTabs();
    }

    private void SetupDefaultTabs()
    {
        SceneViewTab sceneViewTab = new(target, DockSpaceManager.Docks.Middle)
        {
            IsSelected = true,
            TabName = "Scene View",
        };

        SceneHierarchyTab hierarchyTab = new(DockSpaceManager.Docks.Left)
        {
            IsSelected = true,
            TabName = "Hierarchy",
            HeaderWidth = 90f
        };

        GameObjectPropertiesTab propertiesTab = new(DockSpaceManager.Docks.Right)
        {
            IsSelected = true,
            TabName = "Properties"
        };

        ConsoleTab consoleTab = new(DockSpaceManager.Docks.Bottom)
        {
            IsSelected = true,
            TabName = "Console"
        };
    }

    private void EditorCallback()
    {
        DockSpaceManager.DrawDockSpaces();
        TabMenu.DrawTabs(DockSpaceManager);
    }
}