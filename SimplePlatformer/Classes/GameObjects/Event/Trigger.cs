using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.GameObjects.Event;

public class Trigger : GameObject;

public class TriggerScript : MonoBehavior
{
    public BoxCollider Collider = null!;

    public void Awake()
    {
        Collider = GameObject.AddComponent<BoxCollider>();
        Collider.Type = ColliderType.Trigger;
    }
}
