using SimplePlatformer.Classes.GameObjects.Scripts;
using SimplePlatformer.Classes.GameObjects.World;
using ToadEngine.Classes.Base.Rendering.Object;

namespace SimplePlatformer.Classes.GameObjects.Event;

public class SavePoint
{
    public BasePlate SavePointObject { get; private set; }
    public Trigger TGameObject { get; private set; }

    private static int _index;

    public SavePoint(Vector3 pos, Vector3? size = null)
    {
        size ??= new Vector3(1f, 0.1f, 1f);
        Load((Vector3)size, pos);
    }

    public void Load(Vector3 size, Vector3 pos)
    {
        SavePointObject = new BasePlate(size: size);
        SavePointObject.GameObject.Transform.Position = pos;
        SavePointObject.GameObject.Transform.LocalScale = size;

        TGameObject = new Trigger(SavePointObject.GameObject.Transform.LocalScale, SavePointObject.GameObject.Transform.Position,
            $"savePoint_{_index++}");
        
        SavePointObject.GameObject.AddChild(TGameObject.GameObject);
        TGameObject.GameObject.Transform.LocalPosition.Y += 0.45f;
    }

    public T AddScript<T>() where T : new() => TGameObject.AddScript<T>();

    public List<GameObject> GameObjects()
    {
        return [SavePointObject.GameObject, TGameObject.GameObject];
    }
}
