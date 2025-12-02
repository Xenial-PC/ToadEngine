using Guinevere;
using ToadEngine.Classes.Base.Audio;
using ToadEngine.Classes.Base.Physics;
using ToadEngine.Classes.Base.Raycasting;
using ToadEngine.Classes.Base.Rendering.Object;
using MouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;
using Scene = ToadEngine.Classes.Base.Rendering.SceneManagement.Scene;

namespace ToadEngine.Classes.Base.Scripting.Base;

public abstract class Behavior : RenderObject, ICloneable
{
    public RaycastManager Raycast = new(Service.Scene.PhysicsManager.BufferPool);
    public static Dictionary<int, GameObject> BodyToGameObject = new();
    public GameObject GameObject = null!;

    public Scene Scene => Service.Scene;
    public NativeWindow WHandler => Service.NativeWindow;

    public AudioManager AudioManger => Service.Scene.AudioManager;
    public PhysicsManager PhysicsManager => Service.Scene.PhysicsManager;

    public int GetSound(string name) => AudioManger.GetSound(name);
    public Dictionary<string, Source> Sources = new();

    public Source? GetSource(string name) => Sources.GetValueOrDefault(name);

    public Gui UI => GUI.Paint;

    static Behavior()
    {
        Trigger.OnEnter += (a, b) =>
        {
            if (!BodyToGameObject.TryGetValue(a, out var objA) ||
                !BodyToGameObject.TryGetValue(b, out var objB)) return;

            foreach (var component in objA.Component.GetOfType<Behavior>())
                component?.OnTriggerEnter(objB);

            foreach (var component in objB.Component.GetOfType<Behavior>())
                component?.OnTriggerEnter(objA);
        };

        Trigger.OnExit += (a, b) =>
        {
            if (!BodyToGameObject.TryGetValue(a, out var objA) ||
                !BodyToGameObject.TryGetValue(b, out var objB)) return;

            foreach (var component in objA.Component.GetOfType<Behavior>())
                component?.OnTriggerExit(objB);

            foreach (var component in objB.Component.GetOfType<Behavior>())
                component?.OnTriggerExit(objA);
        };
    }

    public virtual void OnGUI() { }
    public virtual void OnFixedUpdate() { }

    public virtual void OnTriggerEnter(GameObject other) { }
    public virtual void OnTriggerExit(GameObject other) { }
    
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
