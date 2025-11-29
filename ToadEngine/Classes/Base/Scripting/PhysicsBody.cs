using BepuPhysics;
using ToadEngine.Classes.Base.Colliders;
using ToadEngine.Classes.Base.Physics;

namespace ToadEngine.Classes.Base.Scripting;

public class PhysicsBody : Behavior
{
    public override void Setup()
    {
        var bodyHandle = new BodyHandle();
        if (GameObject.Component.Get<BaseCollider>() != null)
        {
            var collider = GameObject.Component.Get<BaseCollider>()!;
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

        var body = Scene.PhysicsManager.Simulation.Bodies.GetBodyReference(GameObject.Physics.BodyHandle);
        GameObject.Transform.Position = new Vector3(body.Pose.Position.X, body.Pose.Position.Y, body.Pose.Position.Z);
        GameObject.Transform.Rotation = new Vector3(body.Pose.Orientation.X, body.Pose.Orientation.Y, body.Pose.Orientation.Z);
    }
}
