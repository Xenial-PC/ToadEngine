using ToadEditor.Classes.EditorCore.GUI.Base;
using ToadEditor.Classes.EditorCore.Modules;
using ToadEditor.Classes.EditorCore.Renderer;
using ToadEditor.Classes.EditorCore.Scenes;
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

        SceneManager.Register<TestScene>("test");
        LoadScene("test");

        HookManager.SetupHooks();
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
