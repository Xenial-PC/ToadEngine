using BepuPhysics;
using ToadEngine.Classes.Base.Colliders;
using ToadEngine.Classes.Base.Physics;

namespace ToadEngine.Classes.Base.Scripting;

public class Physics : Behaviour
{
    public override void Setup()
    {
        base.Setup();
        var bodyHandle = new BodyHandle();

        if (GameObject.GetComponent<BoxCollider>() != null)
        {
            var collider = GameObject.GetComponent<BoxCollider>()!;
            bodyHandle = collider.Collider;
        }

        if (GameObject.GetComponent<CapsuleCollider>() != null)
        {
            var collider = GameObject.GetComponent<CapsuleCollider>()!;
            bodyHandle = collider.Collider;
        }

        if (GameObject.GetComponent<CylinderCollider>() != null)
        {
            var collider = GameObject.GetComponent<CylinderCollider>()!;
            bodyHandle = collider.Collider;
        }

        if (GameObject.GetComponent<SphereCollider>() != null)
        {
            var collider = GameObject.GetComponent<SphereCollider>()!;
            bodyHandle = collider.Collider;
        }

        if (GameObject.GetComponent<MeshCollider>() != null)
        {
            var collider = GameObject.GetComponent<MeshCollider>()!;
            bodyHandle = collider.Collider;
        }

        GameObject.Physics = new PhysicsComponent()
        {
            BodyHandle = bodyHandle,
            IsDynamic = true
        };
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (!GameObject.UsePhysics) return;

        var body = GetCurrentScene().PhysicsManager.Simulation.Bodies.GetBodyReference(GameObject.Physics.BodyHandle);
        GameObject.Transform.Position = new Vector3(body.Pose.Position.X, body.Pose.Position.Y, body.Pose.Position.Z);
        GameObject.Transform.Rotation = new Vector3(body.Pose.Orientation.X, body.Pose.Orientation.Y, body.Pose.Orientation.Z);
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}
