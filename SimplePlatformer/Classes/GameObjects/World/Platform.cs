using SimplePlatformer.Classes.GameObjects.Event;
using SimplePlatformer.Classes.GameObjects.Models;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.GameObjects.World;

public class Platform
{
    public TexturedCube GameObject { get; private set; }
    public Trigger TGameObject { get; private set; }

    public Behavior? Script = null;

    private static int _index;

    public Platform(Vector3 size)
    {
        Load(size);
    }

    public void Load(Vector3 size)
    {
        GameObject = new TexturedCube(
            diffuse: $"{Directory.GetCurrentDirectory()}/Resources/Textures/granite.jpg",
            specular: $"{Directory.GetCurrentDirectory()}/Resources/Textures/granite_specular.jpg",
            normal: $"{Directory.GetCurrentDirectory()}/Resources/Textures/granite_normal.png");

        GameObject.Transform.LocalScale = size;
        GameObject.AddComponent<BoxCollider>().Type = ColliderType.Kinematic;
    }

    public List<GameObject> GameObjects()
    {
        if (Script == null) return [GameObject];
        TGameObject = new Trigger(GameObject.Transform.LocalScale, GameObject.Transform.Position,
            $"platform_{_index++}", Script);

        GameObject.AddChild(TGameObject.GameObject);

        var clone = (Script.Clone() as Behavior);
        clone!.GameObject = GameObject;

        GameObject.AddComponent(clone);

        TGameObject.GameObject.Transform.LocalPosition.Y += 0.35f;
        return [GameObject, TGameObject.GameObject];
    }
}
