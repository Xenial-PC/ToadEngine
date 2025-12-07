using BepuPhysics.Collidables;
using BepuPhysics.Trees;
using BepuPhysics;
using BepuUtilities.Memory;
using static ToadEngine.Classes.Base.Raycasting.RaycastManager;
using System.Runtime.CompilerServices;
using ToadEngine.Classes.Base.Physics.Managers;

namespace ToadEngine.Classes.Base.Raycasting;

public unsafe struct HitHandler : IRayHitHandler
{
    public Buffer<RayHit> Hits;
    public int* IntersectionCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AllowTest(CollidableReference collidable)
    {
        return !TriggerRegistry.IsTrigger(collidable.RawHandleValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AllowTest(CollidableReference collidable, int childIndex)
    {
        return !TriggerRegistry.IsTrigger(collidable.RawHandleValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnRayHit(in RayData ray, ref float maximumT, float t, in System.Numerics.Vector3 normal, CollidableReference collidable,
        int childIndex)
    {
        maximumT = t;
        ref var hit = ref Hits[ray.Id];

        if (t > hit.Distance) return;
        if (hit.Distance is float.MaxValue) ++*IntersectionCount;

        hit.Normal = normal;
        hit.Distance = t;
        hit.Collidabe = collidable;
        hit.IsHit = true;
    }
}
