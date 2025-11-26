using BepuPhysics;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Colliders;

public class BoxCollider : Behaviour
{
    public OpenTK.Mathematics.Vector3 Size = OpenTK.Mathematics.Vector3.Zero;
    public BodyHandle Collider;
    public int Handle;
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
        if (Size == OpenTK.Mathematics.Vector3.Zero) 
            Size = GameObject.Transform.LocalScale;
        
        switch (Type)
        {
            case ColliderType.Trigger:
                Collider = GetCurrentScene().PhysicsManager.CreateTriggerBox(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Kinematic:
                Collider = GetCurrentScene().PhysicsManager.CreateKinematicBox(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Dynamic:
                Collider = GetCurrentScene().PhysicsManager.CreateBox(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Static:
                var h = GetCurrentScene().PhysicsManager.CreateStaticBox(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size);

                Handle = h.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
        }
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        var simulation = GetCurrentScene().PhysicsManager.Simulation;
        var body = simulation.Bodies.GetBodyReference(Collider);

        if (Type == ColliderType.Static) return;

        if (Type is ColliderType.Kinematic or ColliderType.Trigger && !GameObject.IsChild)
        {
            body.Pose.Position = (Vector3)GameObject.Transform.Position;

            var finalOrientation = Quaternion.Normalize(GameObject.Transform.Rotation.ToEuler());

            body.Pose.Orientation = finalOrientation;
            body.UpdateBounds();

            return;
        }

        if (GameObject.IsChild && Type is ColliderType.Kinematic or ColliderType.Trigger)
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
