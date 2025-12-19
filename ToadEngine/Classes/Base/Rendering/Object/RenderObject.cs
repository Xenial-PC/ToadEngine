using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Rendering.Object;

public class RenderObject : MonoBehavior
{
    public virtual void Setup() { }
    public virtual void Draw() { }
    public virtual void Resize(FramebufferResizeEventArgs e) { }
}
