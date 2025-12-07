using ToadEngine.Classes.Base.Colliders;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Scripting;

public class PhysicsBody : Behavior
{
    public BaseCollider Collider = null!;

    public void Start()
    {
        if (GameObject.GetComponent<BaseCollider>() == null) return;
        Collider = GameObject.GetComponent<BaseCollider>()!;
    }

    public void Update()
    {
        if (!GameObject.UsePhysics) return;

        var body = Service.Physics.Simulation.Bodies.GetBodyReference(Collider.Collider);
        GameObject.Transform.Position = new Vector3(body.Pose.Position.X, body.Pose.Position.Y, body.Pose.Position.Z);
        GameObject.Transform.Rotation = new Vector3(body.Pose.Orientation.X, body.Pose.Orientation.Y, body.Pose.Orientation.Z);
    }
}