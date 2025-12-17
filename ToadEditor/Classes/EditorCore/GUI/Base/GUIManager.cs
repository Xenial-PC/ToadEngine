using ToadEditor.Classes.EditorCore.GUI.Components;
using ToadEditor.Classes.EditorCore.GUI.Elements;
using ToadEditor.Classes.EditorCore.GUI.Elements.Tabs;
using ToadEditor.Classes.EditorCore.Renderer;

namespace ToadEditor.Classes.EditorCore.GUI.Base;

public class GUIManager(EditorRenderTarget target)
{
    public DockSpaceManager DockSpaceManager = new();
    public HeaderContainer Header = null!;

    public void Setup()
    {
        SetupDefaults();
        ToadEngine.Classes.Base.UI.GUI.GuiCallBack += EditorCallback;
    }

    private void SetupDefaults()
    {
        DockSpaceManager.InitDockSpaces();
        Header = new HeaderContainer(DockSpaceManager);
        SetupDefaultTabs();
    }

    private void SetupDefaultTabs()
    {
        _ = new SceneViewTab(target, DockType.Middle) { IsSelected = true };
        _ = new ConsoleTab(DockType.Bottom) { IsSelected = false };
        _ = new SceneHierarchyTab(DockType.Left) { IsSelected = true };
        _ = new GameObjectPropertiesTab(DockType.Right) { IsSelected = true };
    }

    private void EditorCallback()
    {
        Header.DrawHeader();
        TabMenu.DrawTabs(DockSpaceManager);
    }
}