using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEditor.Classes.EditorCore.Modules.Hooks;

public abstract class UpdateHook
{
    public Scene? Scene => Service.Scene;

    protected UpdateHook()
    {
        if (Scene == null) return;
        Scene.PreUpdate += PreUpdate;
        Scene.PostUpdate += PostUpdate;
    }

    public virtual void PreUpdate()
    {

    }

    public virtual void PostUpdate()
    {

    }
}
