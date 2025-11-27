using SimplePlatformer.Classes.GameObjects.Models;
using ToadEngine.Classes.Base.Scripting;

namespace SimplePlatformer.Classes.GameObjects.Event;

public class Lava
{
    public TexturedCubeModel GameObject { get; private set; }
    public Trigger TGameObject { get; private set; }

    private static int _lava;

    public Behavior Behavior { get; private set; }

    public Lava(Vector3 size, Vector3 position, Behavior behavior)
    {
        Behavior = (behavior.Clone() as Behavior)!;
        Load(size, position, Behavior);
    }

    public void Load(Vector3 size, Vector3 position, Behavior behavior)
    {
        GameObject = new TexturedCubeModel(
            diffuse: $"{Directory.GetCurrentDirectory()}/Resources/Textures/lava.jpg",
            specular: $"{Directory.GetCurrentDirectory()}/Resources/Textures/lava_specular.jpg");

        GameObject.Material.Shininess = 10f;
        GameObject.Transform.LocalScale = size;
        GameObject.Transform.Position = position;

        GameObject.AddComponent<BoxCollider>().Type = ColliderType.Kinematic;
        TGameObject = new Trigger(GameObject.Transform.LocalScale, GameObject.Transform.Position,
            $"lava_{_lava++}", behavior);

        GameObject.AddChild(TGameObject.GameObject);
        TGameObject.GameObject.Transform.LocalPosition.Y += 0.05f;
    }

    public List<GameObject> GameObjects()
    {
        return [GameObject, TGameObject.GameObject];
    }
}
