namespace ToadEngine.Classes.Base.Rendering.SceneManagement;

public interface IRenderTarget
{
    int Width { get; }
    int Height { get; }
    public void Bind();
    public void Unbind();
}
