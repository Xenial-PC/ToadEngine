using SimplePlatformer.Classes.GameObjects.Event;
using SimplePlatformer.Classes.GameObjects.Models;
using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.GameObjects.World;

public class Platform
{
    public TexturedCube GameObject { get; private set; }
    public Trigger TGameObject { get; private set; }

    private static int _index;

    public Platform(Vector3 size)
    {
        Load(size);
    }

    public void Load(Vector3 size)
    {
        GameObject = new TexturedCube(AssetManager.GetMaterial("GraniteMat"));
        GameObject.Transform.LocalScale = size;
        GameObject.AddComponent<BoxCollider>().Type = ColliderType.Kinematic;

        TGameObject = new Trigger(GameObject.Transform.LocalScale, GameObject.Transform.Position,
            $"platform_{_index++}");

        TGameObject.GameObject.Transform.LocalPosition.Y += 0.35f;
        GameObject.AddChild(TGameObject.GameObject);
    }

    public T AddScript<T>() where T : new() => TGameObject.AddScript<T>();

    public List<GameObject> GameObjects()
    {
        if (GameObject.Components.Count <= 0) return [GameObject];
        return [GameObject, TGameObject.GameObject];
    }
}
