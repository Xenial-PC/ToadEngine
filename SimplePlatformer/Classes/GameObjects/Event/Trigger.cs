using ToadEngine.Classes.Base.Rendering.Object;

namespace SimplePlatformer.Classes.GameObjects.Event;

public class Trigger
{
    public TriggerGameObject GameObject { get; private set; }
    
    public Trigger(Vector3 size, Vector3 position, string name)
    {
        Load(size, position, name);
    }

    public void Load(Vector3 size, Vector3 position, string name)
    {
        GameObject = new TriggerGameObject() { Name = name };
        
        GameObject.Transform.Position = position;
        GameObject.Transform.LocalScale = size;

        GameObject.AddComponent<BoxCollider>().Type = ColliderType.Trigger;
    }

    public T AddScript<T>() where T : new() => GameObject.AddComponent<T>();

    public class TriggerGameObject : GameObject;
}
