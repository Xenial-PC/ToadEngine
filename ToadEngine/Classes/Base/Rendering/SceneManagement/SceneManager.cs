namespace ToadEngine.Classes.Base.Rendering.SceneManagement;

public static class SceneManager
{
    public static Dictionary<string, Type> RegisteredScenes = new();

    public static void Register<T>(string name) where T : Scene
        => RegisteredScenes[name] = typeof(T);

    public static Scene Create(string name)
        => (Scene)Activator.CreateInstance(RegisteredScenes[name])!;

    public static Scene SetupScene(Scene sceneObject)
    {
        var scene = new Scene
        {
            AssetManager = sceneObject.AssetManager,
            AudioManager = sceneObject.AudioManager,
            Skybox = sceneObject.Skybox,
            DirectionLight = sceneObject.DirectionLight
        };

        foreach (var objectManagerGameObject in sceneObject.ObjectManager.GameObjects)
            scene.Instantiate(objectManagerGameObject.Value);

        foreach (var objectManagerGameObject in sceneObject.ObjectManager.GameObjectsLast)
            scene.Instantiate(objectManagerGameObject.Value, InstantiateType.Late);

        return scene;
    }
}
