using SimplePlatformer.Classes.GameObjects.Event;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.GameObjects.World;

public class PlatformScript : Behavior
{
    public Trigger Trigger = null!;
    public TriggerScript TriggerScript = null!;
    public BoxCollider Collider = null!;

    public void Awake()
    {
        Collider = GameObject.AddComponent<BoxCollider>();
        Collider.Type = ColliderType.Kinematic;

        Trigger = new Trigger();
        Trigger.Transform.LocalScale = GameObject.Transform.LocalScale;
        Trigger.Transform.LocalPosition.Y += 0.35f;
        TriggerScript = Trigger.AddComponent<TriggerScript>();

        GameObject.AddChild(Trigger);
        Scene.Instantiate(Trigger);
    }
}
