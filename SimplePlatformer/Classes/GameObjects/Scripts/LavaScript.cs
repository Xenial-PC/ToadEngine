using SimplePlatformer.Classes.GameObjects.Event;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.GameObjects.Scripts;

public class LavaScript : MonoBehavior
{
    public Trigger Trigger = null!;
    public TriggerScript TriggerScript = null!;
    public RespawnScript RespawnScript = null!;

    public BoxCollider Collider = null!;
    
    public void Awake()
    {
        Trigger = new Trigger();
        Trigger.Transform.LocalScale = GameObject.Transform.LocalScale;

        TriggerScript = Trigger.AddComponent<TriggerScript>();

        Collider = GameObject.AddComponent<BoxCollider>();
        
        GameObject.AddChild(Trigger);
        Trigger.Transform.LocalPosition.Y += 3;

        Scene.Instantiate(Trigger);
    }
}
