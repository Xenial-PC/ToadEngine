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

    public override void OnStart()
    {
        if (Size == Vector2.Zero)
            Size = new Vector2(Radius, GameObject.Transform.LocalScale.Y);

        _lastSize = Size;
        _lastRadius = Radius;

        switch (Type)
        {
            case ColliderType.Trigger:
                Collider = Scene.PhysicsManager.CreateTriggerCylinder((Vector3)GameObject.Transform.Position, Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Kinematic:
                Collider = Scene.PhysicsManager.CreateKinematicCylinder((Vector3)GameObject.Transform.Position, Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Dynamic:
                Collider = Scene.PhysicsManager.CreateCylinder((Vector3)GameObject.Transform.Position, Size, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Static:
            {
                SCollider = Scene.PhysicsManager.CreateStaticCylinder((Vector3)GameObject.Transform.Position, Size);

                Handle = SCollider.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            }
        }
    }

    public void UpdateBoundingBox()
    {
        var shape = new Cylinder(Radius, Size.Y);
        Resize(shape);
    }
}
