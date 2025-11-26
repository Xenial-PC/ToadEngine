using BepuPhysics;
using Vector2 = System.Numerics.Vector2;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;
using ToadEngine.Classes.Extensions;

namespace ToadEngine.Classes.Base.Colliders;

public class CylinderCollider : BaseCollider
{
    public float Radius;
    public Vector2 Size = Vector2.Zero;

    public override void Setup()
    {
        if (Size == Vector2.Zero)
            Size = new Vector2(Radius, GameObject.Transform.LocalScale.Y);

        switch (Type)
        {
            case ColliderType.Trigger:
                Collider = GetCurrentScene().PhysicsManager.CreateTriggerCylinder((Vector3)GameObject.Transform.Position, Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Kinematic:
                Collider = GetCurrentScene().PhysicsManager.CreateKinematicCylinder((Vector3)GameObject.Transform.Position, Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Dynamic:
                Collider = GetCurrentScene().PhysicsManager.CreateCylinder((Vector3)GameObject.Transform.Position, Size, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Static:
            {
                var h = GetCurrentScene().PhysicsManager.CreateStaticCylinder((Vector3)GameObject.Transform.Position, Size);

                Handle = h.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            }
        }
    }
}
