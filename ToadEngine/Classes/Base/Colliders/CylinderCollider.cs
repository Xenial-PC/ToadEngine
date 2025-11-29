using BepuPhysics;
using Vector2 = System.Numerics.Vector2;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;
using ToadEngine.Classes.Extensions;
using BepuPhysics.Collidables;

namespace ToadEngine.Classes.Base.Colliders;

public class CylinderCollider : BaseCollider
{
    public float Radius;
    public Vector2 Size = Vector2.Zero;

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
                Collider = GetCurrentScene().PhysicsManager.CreateTriggerCylinder((Vector3)GameObject.Transform.Position, Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Kinematic:
                Collider = GetCurrentScene().PhysicsManager.CreateKinematicCylinder((Vector3)GameObject.Transform.Position, Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Dynamic:
                Collider = GetCurrentScene().PhysicsManager.CreateCylinder((Vector3)GameObject.Transform.Position, Size, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Static:
            {
                SCollider = GetCurrentScene().PhysicsManager.CreateStaticCylinder((Vector3)GameObject.Transform.Position, Size);

                Handle = SCollider.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            }
        }
    }

    public override void Update(float deltaTime)
    {
        if (_lastSize == Size && _lastRadius == Radius) return;
        _lastSize = Size;
        _lastRadius = Radius;

        ResizeCylinder();
    }

    private void ResizeCylinder()
    {
        var shape = new Cylinder(Radius, Size.Y);
        Resize(shape);
    }
}
