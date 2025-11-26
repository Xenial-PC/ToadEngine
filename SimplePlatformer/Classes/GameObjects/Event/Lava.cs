using SimplePlatformer.Classes.GameObjects.Models;
using ToadEngine.Classes.Base.Scripting;

namespace SimplePlatformer.Classes.GameObjects.Event;

public class Lava
{
    public TexturedCubeModel GameObject { get; private set; }
    public Trigger TGameObject { get; private set; }

    private static int _lava;

    public Behaviour Behaviour { get; private set; }

    public Lava(Vector3 size, Vector3 position, Behaviour behaviour)
    {
        Behaviour = (behaviour.Clone() as Behaviour)!;
        Load(size, position, Behaviour);
    }

    public void Load(Vector3 size, Vector3 position, Behaviour behaviour)
    {
        GameObject = new TexturedCubeModel(
            diffuse: $"{Directory.GetCurrentDirectory()}/Resources/Textures/lava.jpg",
            specular: $"{Directory.GetCurrentDirectory()}/Resources/Textures/lava_specular.jpg");

        GameObject.Material.Shininess = 10f;
        GameObject.Transform.LocalScale = size;
        GameObject.Transform.Position = position;

        var collider = GameObject.AddComponent<BoxCollider>();
        collider.Type = BoxCollider.ColliderType.Kinematic;
        TGameObject = new Trigger(GameObject.Transform.LocalScale, GameObject.Transform.Position,
            $"lava_{_lava++}", behaviour);

        GameObject.AddChild(TGameObject.GameObject);
        TGameObject.GameObject.Transform.LocalPosition.Y += 0.05f;
    }

    public List<GameObject> GameObjects()
    {
        return [GameObject, TGameObject.GameObject];
    }
}
