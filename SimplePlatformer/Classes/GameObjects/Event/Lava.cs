using Assimp;
using SharpVk;
using SimplePlatformer.Classes.GameObjects.Models;
using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Textures;
using Material = ToadEngine.Classes.Base.Assets.Material;

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
        GameObject = new TexturedCube(AssetManager.GetMaterial("LavaMat"));

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
