using BepuPhysics;
using BepuPhysics.Collidables;
using ToadEngine.Classes.Extensions;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Colliders;

public class SphereCollider : BaseCollider
{
    public float Radius;

    public override void Setup()
    {
        switch (Type)
        {
            case ColliderType.Trigger:
                Collider = GetCurrentScene().PhysicsManager.CreateTriggerSphere((Vector3)GameObject.Transform.Position, Radius);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Kinematic:
                Collider = GetCurrentScene().PhysicsManager.CreateKinematicSphere((Vector3)GameObject.Transform.Position, Radius);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Dynamic:
                Collider = GetCurrentScene().PhysicsManager.CreateSphere((Vector3)GameObject.Transform.Position, Radius, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Static:
                var h = GetCurrentScene().PhysicsManager.CreateStaticSphere((Vector3)GameObject.Transform.Position, Radius);

                Handle = h.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
        }
    }
}
