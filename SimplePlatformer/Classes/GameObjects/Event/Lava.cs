using SimplePlatformer.Classes.GameObjects.Models;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.GameObjects.Event;

public class Lava
{
    public TexturedCube GameObject { get; private set; }
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
        GameObject = new TexturedCube(
            diffuse: $"{Directory.GetCurrentDirectory()}/Resources/Textures/lava.jpg",
            specular: $"{Directory.GetCurrentDirectory()}/Resources/Textures/lava_specular.jpg",
            normal: $"{Directory.GetCurrentDirectory()}/Resources/Textures/lava_normal.png");

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
