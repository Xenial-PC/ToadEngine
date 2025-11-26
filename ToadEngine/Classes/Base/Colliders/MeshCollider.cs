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

    public override void Setup()
    {
        base.Setup();
        if (Size == OpenTK.Mathematics.Vector3.Zero)
            Size = GameObject.Transform.LocalScale;

        switch (Type)
        {
            case ColliderType.Trigger:
                Collider = GetCurrentScene().PhysicsManager.CreateTriggerMesh(
                    (Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Kinematic:
                Collider = GetCurrentScene().PhysicsManager.CreateKinematicMesh((Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Dynamic:
                Collider = GetCurrentScene().PhysicsManager.CreateMesh((Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)Size, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Static:
            {
                var h = GetCurrentScene().PhysicsManager.CreateStaticMesh((Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)Size);

                Handle = h.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            }
        }
    }
}
