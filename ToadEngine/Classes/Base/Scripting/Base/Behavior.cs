using Guinevere;
using ToadEngine.Classes.Base.Audio;
using ToadEngine.Classes.Base.Physics;
using ToadEngine.Classes.Base.Raycasting;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Shaders;
using Scene = ToadEngine.Classes.Base.Rendering.SceneManagement.Scene;

namespace ToadEngine.Classes.Base.Scripting.Base;

public abstract class Behavior : ICloneable
{
    public RaycastManager Raycast = new(Service.Physics.BufferPool);
    public static Dictionary<int, GameObject> BodyToGameObject = new();
    public GameObject GameObject = null!;

    public Scene Scene => Service.Scene;
    public NativeWindow WHandler => Service.NativeWindow;

    public AudioManager AudioManger => Service.Scene.AudioManager;
    public PhysicsSimulation Physics => Service.Physics;

    public int GetSound(string name) => AudioManger.GetSound(name);
    public Dictionary<string, Source> Sources = new();

    public Source? GetSource(string name) => Sources.GetValueOrDefault(name);

    public Gui UI => GUI.Paint;
    public Shader CoreShader => Service.CoreShader;
    
    public static void SetupTriggers()
    {
        TriggerManager.OnEnter += (a, b) =>
        {
            if (!BodyToGameObject.TryGetValue(a, out var objA) ||
                !BodyToGameObject.TryGetValue(b, out var objB)) return;

            foreach (var component in objA.Component.GetOfType<Behavior>())
                component?.OnTriggerEnterMethod?.Invoke(objB);

            foreach (var component in objB.Component.GetOfType<Behavior>())
                component?.OnTriggerEnterMethod?.Invoke(objA);
        };

        TriggerManager.OnExit += (a, b) =>
        {
            if (!BodyToGameObject.TryGetValue(a, out var objA) ||
                !BodyToGameObject.TryGetValue(b, out var objB)) return;

            foreach (var component in objA.Component.GetOfType<Behavior>())
                component?.OnTriggerExitMethod?.Invoke(objB);

            foreach (var component in objB.Component.GetOfType<Behavior>())
                component?.OnTriggerExitMethod?.Invoke(objA);
        };
    }

    internal Action? AwakeMethod;
    internal Action? StartMethod;

    internal Action? UpdateMethod;
    internal Action? FixedUpdateMethod;
    internal Action? OnGuiMethod;

    internal Action<GameObject>? OnTriggerEnterMethod;
    internal Action<GameObject>? OnTriggerExitMethod;

    internal Action<FramebufferResizeEventArgs>? OnResizeMethod;
    internal Action? DisposeMethod;

    public void LoadScene(string name) => Service.Window.LoadScene(name);

    public HitInfo SendRay(Vector3 origin, Vector3 direction, float maxT = 1000)
    {
        var ray = Raycast.SendRay(origin, direction, maxT);
        BodyToGameObject.TryGetValue(ray.Collidabe.RawHandleValue, out var gameObject);

        if (gameObject == null)
            return new HitInfo()
            {
                GameObject = null,
                Hit = ray
            };

        return new HitInfo()
        {
            GameObject = gameObject,
            Hit = ray
        };
    }

    public List<HitInfo> SendRays(List<RaycastManager.Ray> rays)
    {
        var hits = new List<HitInfo>();
        var ray = Raycast.SendRays(rays);
        foreach (var hit in ray)
        {
            BodyToGameObject.TryGetValue(hit.Collidabe.RawHandleValue, out var gameObject);
            if (gameObject == null)
            {
                hits.Add(new HitInfo()
                {
                    GameObject = new GameObject(),
                    Hit = hit
                });
                continue;
            }

            hits.Add(new HitInfo()
            {
                GameObject = gameObject,
                Hit = hit
            });
        }
        return hits;
    }

    public struct HitInfo
    {
        public GameObject? GameObject;
        public RaycastManager.RayHit Hit;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
