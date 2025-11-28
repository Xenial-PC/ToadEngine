namespace ToadEngine.Classes.Base.Rendering;

public static class SceneManager
{
    public static Dictionary<string, Type> RegisteredScenes = new();

    public static void Register<T>(string name) where T : Scene
        => RegisteredScenes[name] = typeof(T);

    public static Scene Create(string name)
        => (Scene)Activator.CreateInstance(RegisteredScenes[name])!;
}
