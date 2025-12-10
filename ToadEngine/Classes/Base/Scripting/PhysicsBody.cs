using BepuPhysics;
using ToadEngine.Classes.Base.Colliders;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Scripting;

public class PhysicsBody : Behavior
{
    public BaseCollider Collider = null!;
    public BodyReference GetBody => Service.Physics.Simulation.Bodies.GetBodyReference(Collider.Collider);

    public void Start()
    {
        if (GameObject.GetComponent<BaseCollider>() == null) return;
        Collider = GameObject.GetComponent<BaseCollider>()!;
    }

    public void Update()
    {
        if (!GameObject.UsePhysics) return;

        var body = GetBody;
        GameObject.Transform.Position = new Vector3(body.Pose.Position.X, body.Pose.Position.Y, body.Pose.Position.Z);
        GameObject.Transform.Rotation = new Vector3(body.Pose.Orientation.X, body.Pose.Orientation.Y, body.Pose.Orientation.Z);
    }

    public void ApplyForce(Vector3 linear = default, Vector3 angular = default)
    {
        if (linear != default) GetBody.Velocity.Linear += (System.Numerics.Vector3)linear;
        if (angular != default) GetBody.Velocity.Angular += (System.Numerics.Vector3)angular;
    }

    public void ApplyDrag(Vector3 linearDrag) => GetBody.Velocity.Linear += (System.Numerics.Vector3)linearDrag;
    public void ApplyAngularDrag(Vector3 angularDrag) => GetBody.Velocity.Angular += (System.Numerics.Vector3)angularDrag;
}