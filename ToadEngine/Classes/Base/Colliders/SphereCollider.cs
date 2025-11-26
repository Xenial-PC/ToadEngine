using BepuPhysics;
using BepuPhysics.Collidables;
using ToadEngine.Classes.Extensions;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Colliders;

public class SphereCollider(float radius, float mass = 1f, bool isKinematic = false, bool isStatic = false, bool isTrigger = false) : Behaviour
{
    public BodyHandle Collider;

    public override void Setup()
    {
        base.Setup();

        if (isTrigger)
        {
            Collider = GetCurrentScene().PhysicsManager.CreateTriggerSphere((Vector3)GameObject.Transform.Position,
                radius);

            BodyToGameObject[Collider.Value] = GameObject;
            return;
        }

        if (isKinematic)
        {
            Collider = GetCurrentScene().PhysicsManager.CreateKinematicSphere((Vector3)GameObject.Transform.Position,
                radius);

            BodyToGameObject[Collider.Value] = GameObject;
            return;
        }

        if (!isStatic)
        {
            Collider = GetCurrentScene().PhysicsManager.CreateSphere((Vector3)GameObject.Transform.Position,
                radius, mass);

            BodyToGameObject[Collider.Value] = GameObject;
            return;
        }

        GetCurrentScene().PhysicsManager.CreateStaticSphere((Vector3)GameObject.Transform.Position,
            radius);
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
