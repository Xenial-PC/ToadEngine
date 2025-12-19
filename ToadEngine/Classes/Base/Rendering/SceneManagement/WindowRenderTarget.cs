using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Rendering.SceneManagement;

public class WindowRenderTarget() : IRenderTarget
{
    public Window.Window Window = Service.Window;

    public int Width => Window.Width;
    public int Height => Window.Height;

    public void Bind()
    {
        if (!Service.Scene.Settings.IsRunning) Service.Scene.Settings.IsRunning = true;
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.Viewport(0, 0, Width, Height);
    }

    public void Unbind() { }
}
