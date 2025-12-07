namespace ToadEngine.Classes.Base.Rendering.Object;

public interface IRenderObject
{
    public void Setup() { }
    public void Draw() { }
    public void Resize(FramebufferResizeEventArgs e) { }
}
