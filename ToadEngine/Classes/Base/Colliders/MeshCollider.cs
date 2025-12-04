using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using ToadEngine.Classes.Extensions;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Colliders;

public class MeshCollider : BaseCollider
{
    public OpenTK.Mathematics.Vector3 Size = OpenTK.Mathematics.Vector3.Zero;
    public Buffer<Triangle> Triangles;

    private OpenTK.Mathematics.Vector3 _lastSize;

    public override void OnStart()
    {
        if (Size == OpenTK.Mathematics.Vector3.Zero)
            Size = GameObject.Transform.LocalScale;

        _lastSize = Size;

        switch (Type)
        {
            case ColliderType.Trigger:
                Collider = Scene.PhysicsManager.CreateTriggerMesh(
                    (Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Kinematic:
                Collider = Scene.PhysicsManager.CreateKinematicMesh((Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Dynamic:
                Collider = Scene.PhysicsManager.CreateMesh((Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)Size, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Static:
            {
                SCollider = Scene.PhysicsManager.CreateStaticMesh((Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)Size);

                Handle = SCollider.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            }
        }
    }

    public void UpdateBoundingBox()
    {
        var shape = new Mesh(Triangles, (Vector3)Size, PhysicsManager.BufferPool);
        Resize(shape);
    }
}
