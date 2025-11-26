using BepuPhysics;
using ToadEngine.Classes.Base.Colliders;
using ToadEngine.Classes.Base.Physics;

namespace ToadEngine.Classes.Base.Scripting;

public class PhysicsBody : Behaviour
{
    public override void Setup()
    {
        var bodyHandle = new BodyHandle();
        if (GameObject.GetComponent<BaseCollider>() != null)
        {
            var collider = GameObject.GetComponent<BaseCollider>()!;
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
