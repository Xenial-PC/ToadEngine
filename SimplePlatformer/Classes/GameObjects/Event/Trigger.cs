using SimplePlatformer.Classes.GameObjects.Models;

namespace SimplePlatformer.Classes.GameObjects.Event;

public class Trigger
{
    public TriggerGameObject GameObject { get; private set; }
    
    public Trigger(Vector3 size, Vector3 position, string name, Behaviour scriptBehaviour)
    {
        Load(size, position, name, scriptBehaviour);
    }

    public void Load(Vector3 size, Vector3 position, string name, Behaviour scriptBehaviour)
    {
        GameObject = new TriggerGameObject() { Name = name };
        
        GameObject.Transform.Position = position;
        GameObject.Transform.LocalScale = size;

        GameObject.AddComponent<BoxCollider>().Type = BoxCollider.ColliderType.Trigger;
        GameObject.AddComponent(scriptBehaviour);
    }

    public class TriggerGameObject : GameObject;
}
