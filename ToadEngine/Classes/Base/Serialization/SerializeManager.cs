using Prowl.Echo;
using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Serialization;

public class SerializeManager
{
    private Scene Scene => Service.Scene;

    public List<EchoObject> SerializeGameObjects()
    {
        var serializedList = 
            Scene.ObjectManager.GameObjects.Select(gameObject => Serializer.Serialize(gameObject)).ToList();
        serializedList.AddRange(Scene.ObjectManager.GameObjectsLast.Select(gameObject => Serializer.Serialize(gameObject)));
        return serializedList;
    }

    public EchoObject SerializeScene => Scene.Serialized;
}
