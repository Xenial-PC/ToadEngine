using BepuPhysics;
using ToadEngine.Classes.Base.Colliders;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Scripting;

public class PhysicsBody : MonoBehavior
{
    public Collider Collider = null!;
    public BodyReference GetBodyRef => Service.Physics.Simulation.Bodies.GetBodyReference(Collider.BHandle);

    public void Start()
    {
        if (GameObject.GetComponent<Collider>() == null) return;
        Collider = GameObject.GetComponent<Collider>()!;
    }

    public void Update()
    {
        if (!GameObject.UsePhysics) return;

        var body = GetBodyRef;
        GameObject.Transform.Position = new Vector3(body.Pose.Position.X, body.Pose.Position.Y, body.Pose.Position.Z);
        GameObject.Transform.Rotation = new Vector3(body.Pose.Orientation.X, body.Pose.Orientation.Y, body.Pose.Orientation.Z);
    }

    public void ApplyForce(Vector3 linear = default, Vector3 angular = default)
    {
        if (linear != default) GetBodyRef.Velocity.Linear += (System.Numerics.Vector3)linear * Time.FixedTime;
        if (angular != default) GetBodyRef.Velocity.Angular += (System.Numerics.Vector3)angular * Time.FixedTime;
    }

    public void ApplyDrag(Vector3 linearDrag) => GetBodyRef.Velocity.Linear += (System.Numerics.Vector3)linearDrag * Time.FixedTime;
    public void ApplyAngularDrag(Vector3 angularDrag) => GetBodyRef.Velocity.Angular += (System.Numerics.Vector3)angularDrag * Time.FixedTime;
}