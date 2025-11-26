using Assimp;
using BepuPhysics;
using BepuUtilities.Memory;
using Guinevere;
using ToadEngine.Classes.Base.Audio;
using ToadEngine.Classes.Base.Physics;
using ToadEngine.Classes.Base.Raycasting;
using MouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;
using Scene = ToadEngine.Classes.Base.Rendering.Scene;

namespace ToadEngine.Classes.Base.Scripting;

public abstract class Behavior : RenderObject, ICloneable
{
    public RaycastManager Raycast = new(GetCurrentScene().PhysicsManager.BufferPool);
    public static readonly Dictionary<int, GameObject> BodyToGameObject = new();
    public GameObject GameObject = null!;

    public Scene Scene => GetCurrentScene();
    public AudioManager AudioManger => GetCurrentScene().AudioManager;
    public PhysicsManager PhysicsManager => GetCurrentScene().PhysicsManager;

    public int GetSound(string name) => AudioManger.GetSound(name);
    public Dictionary<string, Source> Sources = new();

    public Source? GetSource(string name) => Sources.GetValueOrDefault(name);

    public Gui UI => GUI.Paint;

    public float DeltaTime;

    static Behavior()
    {
        Trigger.OnEnter += (a, b) =>
        {
            if (!BodyToGameObject.TryGetValue(a, out var objA) ||
                !BodyToGameObject.TryGetValue(b, out var objB)) return;

            foreach (var component in objA.GetComponents<Behavior>())
                component?.OnTriggerEnter(objB);

            foreach (var component in objB.GetComponents<Behavior>())
                component?.OnTriggerEnter(objA);
        };

        Trigger.OnExit += (a, b) =>
        {
            if (!BodyToGameObject.TryGetValue(a, out var objA) ||
                !BodyToGameObject.TryGetValue(b, out var objB)) return;

            foreach (var component in objA.GetComponents<Behavior>())
                component?.OnTriggerExit(objB);

            foreach (var component in objB.GetComponents<Behavior>())
                component?.OnTriggerExit(objA);
        };
    }

    public virtual void OnGUI() {}
    public virtual void OnTriggerEnter(GameObject other) {}
    public virtual void OnTriggerExit(GameObject other) {}

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

    public static class Input
    {
        public static bool IsKeyDown(Keys key)
        {
            return WHandler.KeyboardState.IsKeyDown(key);
        }

        public static bool IsKeyReleased(Keys key)
        {
            return WHandler.KeyboardState.IsKeyReleased(key);
        }

        public static bool IsKeyPressed(Keys key)
        {
            return WHandler.KeyboardState.IsKeyPressed(key);
        }

        public static bool IsMouseDown(MouseButton button)
        {
            return WHandler.MouseState.IsButtonDown(button);
        }

        public static bool IsMousePressed(MouseButton button)
        {
            return WHandler.MouseState.IsButtonPressed(button);
        }

        public static bool IsMouseReleased(MouseButton button)
        {
            return WHandler.MouseState.IsButtonReleased(button);
        }

        public static Vector2 GetMousePos()
        {
            return WHandler.MouseState.Position;
        }
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
