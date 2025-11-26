using BepuPhysics;
using Vector2 = System.Numerics.Vector2;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;
using ToadEngine.Classes.Extensions;

namespace ToadEngine.Classes.Base.Colliders;

public class CylinderCollider(float radius, float mass = 1f, bool isKinematic = false, bool isStatic = false, bool isTrigger = false) : Behaviour
{
    public BodyHandle Collider;
    public int Handle;

    public override void Setup()
    {
        base.Setup();

        if (isTrigger)
        {
            Collider = GetCurrentScene().PhysicsManager.CreateTriggerCylinder((Vector3)GameObject.Transform.Position,
                new Vector2(radius, GameObject.Transform.LocalScale.Y));

            Handle = Collider.Value;
            BodyToGameObject[Handle] = GameObject;
            return;
        }

        if (isKinematic)
        {
            Collider = GetCurrentScene().PhysicsManager.CreateKinematicCylinder((Vector3)GameObject.Transform.Position,
                new Vector2(radius, GameObject.Transform.LocalScale.Y));

            Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
        }

        if (!isStatic)
        {
            Collider = GetCurrentScene().PhysicsManager.CreateCylinder((Vector3)GameObject.Transform.Position,
                new Vector2(radius, GameObject.Transform.LocalScale.Y), mass);

            Handle = Collider.Value;
            BodyToGameObject[Handle] = GameObject;
            return;
        }

        var h = GetCurrentScene().PhysicsManager.CreateStaticCylinder((Vector3)GameObject.Transform.Position,
            new Vector2(radius, GameObject.Transform.LocalScale.Y), mass);

        Handle = h.Value;
        BodyToGameObject[Handle] = GameObject;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        var simulation = GetCurrentScene().PhysicsManager.Simulation;
        var body = simulation.Bodies.GetBodyReference(Collider);

        if (isStatic) return;

        if (isKinematic && !GameObject.IsChild)
        {
            body.Pose.Position = (Vector3)GameObject.Transform.Position;

            var finalOrientation = Quaternion.Normalize(GameObject.Transform.Rotation.ToEuler());

            body.Pose.Orientation = finalOrientation;
            body.UpdateBounds();

            return;
        }

        if (GameObject.IsChild && isKinematic)
        {
            body.Pose.Position = (Vector3)GameObject.Transform.Position + (Vector3)GameObject.Transform.LocalPosition;

            var worldRot = GameObject.Transform.Rotation;
            var localRot = GameObject.Transform.LocalRotation;

            var finalOrientation = Quaternion.Normalize(worldRot.ToEuler() * localRot.ToEuler());

            body.Pose.Orientation = finalOrientation;
            body.UpdateBounds();

            return;
        }

        if (GameObject.UsePhysics)
        {
            var pose = body.Pose;
            var q = pose.Orientation;

            GameObject.Transform.Position = new OpenTK.Mathematics.Vector3(
                pose.Position.X, pose.Position.Y, pose.Position.Z);

            GameObject.Transform.Rotation = new OpenTK.Mathematics.Vector3(
                q.X, q.Y, q.Z);

            return;
        }

        // GameObject doesn't use physics
        body.Pose.Position = (Vector3)GameObject.Transform.Position;
        body.Pose.Orientation = GameObject.Transform.Rotation.ToEuler();
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}
