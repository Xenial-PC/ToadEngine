using ToadEngine.Classes.Base.Raycasting;
using ToadEngine.Classes.Base.Rendering.Object;
using UltraMapper;

namespace ToadEngine.Classes.Base.Scripting.Base;

public class Behavior : Component
{
    internal Action? AwakeMethod;
    internal Action? StartMethod;

    internal Action? UpdateMethod;
    internal Action? FixedUpdateMethod;
    internal Action? OnGuiMethod;

    internal Action<GameObject>? OnTriggerEnterMethod;
    internal Action<GameObject>? OnTriggerExitMethod;

    internal Action<FramebufferResizeEventArgs>? OnResizeMethod;
    internal Action? DisposeMethod;

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
        var mapper = new Mapper();
        var clone = mapper.Map(this);

        return clone;
    }
}
