using SimplePlatformer.Classes.GameObjects.Models;
using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Rendering.Object;

namespace SimplePlatformer.Classes.GameObjects.Event;

public class Lava
{
    public TexturedCube GameObject { get; private set; }
    public Trigger TGameObject { get; private set; }

    private static int _lava;

    public Lava(Vector3 size, Vector3 position)
    {
        Load(size, position);
    }

    public void Load(Vector3 size, Vector3 position)
    {
        GameObject = new TexturedCube(AssetManager.GetMaterial("LavaMat"));

        GameObject.Transform.LocalScale = size;
        GameObject.Transform.Position = position;

        GameObject.AddComponent<BoxCollider>().Type = ColliderType.Kinematic;
        TGameObject = new Trigger(GameObject.Transform.LocalScale, GameObject.Transform.Position,
            $"lava_{_lava++}");

        GameObject.AddChild(TGameObject.GameObject);
        TGameObject.GameObject.Transform.LocalPosition.Y += 0.05f;
    }

    public T AddScript<T>() where T : new() => TGameObject.AddScript<T>();

    public List<GameObject> GameObjects() => [GameObject, TGameObject.GameObject];
}
