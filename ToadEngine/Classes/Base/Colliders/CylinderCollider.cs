using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using BepuPhysics.Collidables;

namespace ToadEngine.Classes.Base.Colliders;

public class CylinderCollider : BaseCollider
{
    public float Radius;
    public Vector2 Size = Vector2.Zero;

    public override void OnStart()
    {
        if (Size == Vector2.Zero)
            Size = new Vector2(Radius, GameObject.Transform.LocalScale.Y);

        switch (Type)
        {
            case ColliderType.Trigger:
                Collider = Physics.ColliderManager.CreateTrigger.Cylinder((Vector3)GameObject.Transform.Position, Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Kinematic:
                Collider = Physics.ColliderManager.CreateKinematic.Cylinder((Vector3)GameObject.Transform.Position, Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Dynamic:
                Collider = Physics.ColliderManager.CreateDynamic.Cylinder((Vector3)GameObject.Transform.Position, Size, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Static:
            {
                SCollider = Physics.ColliderManager.CreateStatic.Cylinder((Vector3)GameObject.Transform.Position, Size);

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
