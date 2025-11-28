using ToadEngine.Classes.Base.Physics;
using ToadEngine.Classes.Shaders;

namespace ToadEngine.Classes.Base.Rendering;

public abstract class RenderObject
{
    public static Dictionary<Type, object> Services = new();

    private bool _isDisposing, _isSetup;
    public bool IsEnabled = true;

    public static NativeWindow WHandler => GetNativeWindow();

    public virtual void Setup() {}
    public virtual void Draw(float deltaTime) {}
    public virtual void Update(float deltaTime) {}
    public virtual void Resize(FramebufferResizeEventArgs e) {}
    public virtual void Dispose() {}

    public static void AddService<T>(T service) where T : class
    {
        Services[typeof(T)] = service;
    }

    public static void RemoveService<T>(T service) where T : class
    {
        if (!Services.ContainsKey(typeof(T))) return;
        Services.Remove(typeof(T));
    }

    public static T? GetService<T>() where T : class
    {
        return Services[typeof(T)] as T;
    }

    public static T GetCurrentScene<T>() where T : class
    {
        return (GetService<Scene>() as T)!;
    }

    public static Scene GetCurrentScene()
    {
        return (GetService<Scene>())!;
    }

    private static NativeWindow GetNativeWindow()
    {
        return GetService<NativeWindow>()!;
    }

    public static Window.Window GetWindow()
    {
        return GetService<Window.Window>()!;
    }

    public static Shader GetCoreShader()
    {
        var shader = GetService<Shader>();
        return shader!;
    }

    public void OnSetup()
    {
        if (_isSetup) return;
        Setup();
        _isSetup = true;
        _isDisposing = false;
    }

    public void OnDraw(float deltaTime)
    {
        if (_isDisposing || !IsEnabled) return;
        Draw(deltaTime);
    }

    public void OnUpdate(float deltaTime)
    {
        if (_isDisposing || !IsEnabled) return;
        Update(deltaTime);
    }

    public void OnResize(FramebufferResizeEventArgs e)
    {
        Resize(e);
    }

    public void OnDispose()
    {
        _isSetup = false;
        _isDisposing = true;
        Dispose();
    }
}
