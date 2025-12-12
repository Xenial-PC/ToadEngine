using Guinevere;
using ToadEditor.Classes.EditorCore.Renderer;
using ToadEngine.Classes.Base.Scripting.Base;
using MouseButton = Guinevere.MouseButton;
using Vector2 = System.Numerics.Vector2;
using Window = ToadEngine.Classes.Window.Window;

namespace ToadEditor.Classes.EditorCore.GUI;

public class GUIManager(EditorRenderTarget target, Window window)
{
    public SceneViewWindow SceneViewWindow = new SceneViewWindow(target, window);

    public void Setup()
    {
        SetupDefaults();
        ToadEngine.Classes.Base.UI.GUI.GuiCallBack += EditorCallback;
    }

    public void SetupDefaults()
    {
        SceneViewWindow.Setup();
    }

    private void EditorCallback()
    {
        SceneViewWindow.DrawSceneViewWindow();
    }
}