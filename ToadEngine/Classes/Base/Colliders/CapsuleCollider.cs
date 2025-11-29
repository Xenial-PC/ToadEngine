using BepuPhysics;
using Vector2 = System.Numerics.Vector2;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;
using ToadEngine.Classes.Extensions;
using System.Reflection.Metadata;
using BepuPhysics.Collidables;

namespace ToadEngine.Classes.Base.Colliders;

public class CapsuleCollider : BaseCollider
{
    public Vector2 Size = Vector2.Zero;
    public float Radius;

    private float _lastRadius;
    private Vector2 _lastSize;

    public override void Setup()
    {
        if (Size == Vector2.Zero)
            Size = new Vector2(Radius, GameObject.Transform.LocalScale.Y);

        _lastSize = Size;
        _lastRadius = Radius;

        switch (Type)
        {
            case ColliderType.Trigger:
                Collider = GetCurrentScene().PhysicsManager.CreateTriggerCapsule((Vector3)GameObject.Transform.Position, Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Kinematic:
                Collider = GetCurrentScene().PhysicsManager.CreateKinematicCapsule((Vector3)GameObject.Transform.Position, Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Dynamic:
                Collider = GetCurrentScene().PhysicsManager.CreateCapsule((Vector3)GameObject.Transform.Position, Size, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Static:
                var h = GetCurrentScene().PhysicsManager.CreateStaticCapsule((Vector3)GameObject.Transform.Position, Size);

                Handle = h.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
        }
    }

    public override void Update(float deltaTime)
    {
        if (_lastSize == Size && _lastRadius == Radius) return;
        _lastSize = Size;
        _lastRadius = Radius;

        ResizeCapsule();
    }

    private void ResizeCapsule()
    {
        var shape = new Capsule(Radius, Size.Y);
        Resize(shape);
    }
}
