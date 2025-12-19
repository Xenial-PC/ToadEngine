using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEditor.Classes.EditorCore.Modules.Hooks;

public abstract class RenderHook
{
    public Scene? Scene => Service.Scene;

    protected RenderHook()
    {
        if (Scene == null) return;
        Scene.PreRender += PreRender;
        Scene.PostRender += PostRender;
    }

    public virtual void PreRender()
    {

    }

    public virtual void PostRender()
    {

    }
}
