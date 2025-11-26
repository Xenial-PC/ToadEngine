using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using ToadEngine.Classes.Extensions;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Colliders;

public class MeshCollider(Buffer<Triangle> triangles, float mass = 1f, bool isKinematic = false, bool isStatic = false, bool isTrigger = false) : Behaviour
{
    public int Handle;
    public BodyHandle Collider;

    public override void Setup()
    {
        base.Setup();

        if (isTrigger)
        {
            Collider = GetCurrentScene().PhysicsManager.CreateTriggerMesh(
                (Vector3)GameObject.Transform.Position,
                triangles, (Vector3)GameObject.Transform.LocalScale);

            Handle = Collider.Value;
            BodyToGameObject[Handle] = GameObject;
            return;
        }

        if (isKinematic)
        {
            Collider = GetCurrentScene().PhysicsManager.CreateKinematicMesh((Vector3)GameObject.Transform.Position,
                triangles, (Vector3)GameObject.Transform.LocalScale);

            Handle = Collider.Value;
            BodyToGameObject[Handle] = GameObject;
            return;
        }

        if (!isStatic)
        {
            Collider = GetCurrentScene().PhysicsManager.CreateMesh((Vector3)GameObject.Transform.Position,
                triangles, (Vector3)GameObject.Transform.LocalScale, mass);

            Handle = Collider.Value;
            BodyToGameObject[Handle] = GameObject;
            return;
        }

        var h = GetCurrentScene().PhysicsManager.CreateStaticMesh((Vector3)GameObject.Transform.Position,
            triangles, (Vector3)GameObject.Transform.LocalScale, mass);

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
