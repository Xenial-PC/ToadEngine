using SimplePlatformer.Classes.GameObjects.Models;

namespace SimplePlatformer.Classes.GameObjects.Event;

public class Trigger
{
    public TriggerGameObject GameObject { get; private set; }
    
    public Trigger(Vector3 size, Vector3 position, string name, Behavior scriptBehavior)
    {
        Load(size, position, name, scriptBehavior);
    }

    public void Load(Vector3 size, Vector3 position, string name, Behavior scriptBehavior)
    {
        GameObject = new TriggerGameObject() { Name = name };
        
        GameObject.Transform.Position = position;
        GameObject.Transform.LocalScale = size;

        GameObject.AddComponent<BoxCollider>().Type = ColliderType.Trigger;
        GameObject.AddComponent(scriptBehavior);
    }

    public class TriggerGameObject : GameObject;
}
