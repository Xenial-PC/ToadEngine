using ToadEditor.Classes.EditorCore.Modules.Hooks;
using ToadEngine.Classes.Base.Objects.View;

namespace ToadEditor.Classes.EditorCore.Modules;

public class EditorCamera : UpdateHook
{
    public override void PreUpdate()
    {
        //Console.WriteLine(Camera.GameObjects.Count);
        /*var camera = Camera.MainCamera.GameObject;
        camera?.UpdateBehaviors();*/
    }

    public override void PostUpdate()
    {
        
    }
}
