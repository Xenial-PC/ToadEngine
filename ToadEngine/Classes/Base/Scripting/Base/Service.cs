using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Shaders;

namespace ToadEngine.Classes.Base.Scripting.Base;

public static class Service
{
    private static readonly Dictionary<Type, object> Services = new();

    public static T? Get<T>() where T : class => Services[typeof(T)] as T;
    public static void Add<T>(T service) where T : class => Services[typeof(T)] = service;
    public static void Clear() => Services.Clear();

    public static void Remove<T>(T service) where T : class
    {
        if (!Services.ContainsKey(typeof(T))) return;
        Services.Remove(typeof(T));
    }

    public static void Remove<T>() where T : class
    {
        if (!Services.ContainsKey(typeof(T))) return;
        Services.Remove(typeof(T));
    }

    public static T GetSceneAs<T>() where T : class => (Get<Scene>() as T)!;

    public static Scene Scene => Get<Scene>()!;
    public static NativeWindow NativeWindow => Get<NativeWindow>()!;
    public static Window.Window Window => Get<Window.Window>()!;

    public static Shader CoreShader
    {
        get => Get<Shader>()!;
        set
        {
            Remove<Shader>();
            Add(value);
            value.Use();
        }
    }

    public static Camera MainCamera
    {
        get => Get<Camera>()!;
        set
        {
            Remove<Camera>();
            Add(value);
        }
    }
}
