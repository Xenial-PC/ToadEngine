using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using ToadEngine.Classes.Extensions;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Colliders;

public class MeshCollider : Behaviour
{
    public BodyHandle Collider;
    public int Handle;
    private Buffer<Triangle> Triangles;
    public ColliderType Type;
    public float Mass = 1f;

    public enum ColliderType
    {
        Kinematic,
        Static,
        Trigger,
        Dynamic
    }

    public override void Setup()
    {
        base.Setup();
        switch (Type)
        {
            case ColliderType.Trigger:
                Collider = GetCurrentScene().PhysicsManager.CreateTriggerMesh(
                    (Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)GameObject.Transform.LocalScale);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Kinematic:
                Collider = GetCurrentScene().PhysicsManager.CreateKinematicMesh((Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)GameObject.Transform.LocalScale);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Dynamic:
                Collider = GetCurrentScene().PhysicsManager.CreateMesh((Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)GameObject.Transform.LocalScale, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Static:
            {
                var h = GetCurrentScene().PhysicsManager.CreateStaticMesh((Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)GameObject.Transform.LocalScale);

                Handle = h.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            }
        }
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        var simulation = GetCurrentScene().PhysicsManager.Simulation;
        var body = simulation.Bodies.GetBodyReference(Collider);

        if (Type == ColliderType.Static) return;
        if (Type == ColliderType.Kinematic && !GameObject.IsChild)
        {
            body.Pose.Position = (Vector3)GameObject.Transform.Position;

            var finalOrientation = Quaternion.Normalize(GameObject.Transform.Rotation.ToEuler());

            body.Pose.Orientation = finalOrientation;
            body.UpdateBounds();

            return;
        }

        if (GameObject.IsChild && Type == ColliderType.Kinematic)
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
