using SimplePlatformer.Classes.Scenes;
using Window = ToadEngine.Classes.Window.Window;

namespace SimplePlatformer.Classes.Base;

public class SPlatFormerWindow(int width, int height, string title) : Window(width, height, title)
{
    private bool _isFullScreen;
    
    public LevelOne LevelOne { get; set; }
    public LevelTwo LevelTwo { get; set; }

    public override void Setup()
    {
        base.Setup();
        VSync = VSyncMode.On;

        LevelOne = new LevelOne();
        LevelTwo = new LevelTwo();

        LoadScene("Level1");
    }

    public override void OnInit()
    {
        base.OnInit();
        CursorState = CursorState.Grabbed;
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
        base.OnDraw(e);
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
