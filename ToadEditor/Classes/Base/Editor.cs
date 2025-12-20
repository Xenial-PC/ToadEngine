using ToadEditor.Classes.EditorCore.GUI.Base;
using ToadEditor.Classes.EditorCore.Modules;
using ToadEditor.Classes.EditorCore.Renderer;
using ToadEditor.Classes.EditorCore.Scenes;
using ToadEngine.Classes.Base.Objects.BuiltIn;
using ToadEngine.Classes.Base.Rendering.SceneManagement;
using Window = ToadEngine.Classes.Window.Window;

namespace ToadEditor.Classes.Base;

public class Editor(int width, int height, string title) : Window(width, height, title)
{
    private bool _isFullScreen;
    public static GUIManager GuiManager = null!;

    public EditorRenderTarget EditorRenderTarget => (RenderTarget as EditorRenderTarget)!;

    public override void Setup()
    {
        base.Setup();
        VSync = VSyncMode.On;

        RenderTarget = new EditorRenderTarget();
        GuiManager = new(EditorRenderTarget);
        GuiManager.Setup();

        var scene = new Scene();

        var cube = BuiltIn.Primitives.Cube;
        cube.Name = "Parent Cube";
        cube.Transform.Position = new Vector3(0f, 0f, -1f);
        //cube.AddComponent<FallScript>();

        var childCube = BuiltIn.Primitives.Cube;
        childCube.Name = "Child Cube";
        childCube.Transform.LocalPosition.Y += 2f;

        var childCube2 = BuiltIn.Primitives.Cube;
        childCube2.Name = "Child Cube 2";
        childCube2.Transform.LocalPosition.Y += 2f;

        var childCube3 = BuiltIn.Primitives.Cube;
        childCube3.Name = "Child Cube 3";
        childCube3.Transform.LocalPosition.Y += 2f;

        cube.AddChild(childCube);
        cube.AddChild(childCube3);

        childCube.AddChild(childCube2);

        scene.Instantiate(cube);

        LoadScene(scene);

        //HookManager.SetupHooks();
    }

    public override void OnInit()
    {
        CursorState = CursorState.Normal;
    }

    public override void OnUpdate(FrameEventArgs e)
    {
        base.OnUpdate(e);
        if (!IsFocused) return;

        if (KeyboardState.IsKeyPressed(Keys.F11))
        {
            _isFullScreen = !_isFullScreen;
            WindowState = _isFullScreen ? WindowState.Fullscreen : WindowState.Maximized;
        }
    }

    public override void OnDraw(FrameEventArgs e)
    {
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
    }

    public override void OnDispose()
    {
        base.OnDispose();
    }
}
