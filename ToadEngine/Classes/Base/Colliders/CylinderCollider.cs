using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using BepuPhysics.Collidables;

namespace ToadEngine.Classes.Base.Colliders;

public class CylinderCollider : Collider
{
    public float Radius;
    public Vector2 Size = Vector2.Zero;

    public void Awake()
    {
        if (Size == Vector2.Zero)
            Size = new Vector2(Radius, GameObject.Transform.LocalScale.Y);

        switch (Type)
        {
            case ColliderType.Trigger:
                BHandle = Physics.ColliderManager.CreateTrigger.Cylinder((Vector3)GameObject.Transform.Position, Size);

                Handle = BHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Kinematic:
                BHandle = Physics.ColliderManager.CreateKinematic.Cylinder((Vector3)GameObject.Transform.Position, Size);

                Handle = BHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Dynamic:
                BHandle = Physics.ColliderManager.CreateDynamic.Cylinder((Vector3)GameObject.Transform.Position, Size, Mass);

                Handle = BHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Static:
            {
                SHandle = Physics.ColliderManager.CreateStatic.Cylinder((Vector3)GameObject.Transform.Position, Size);

                Handle = SHandle.Value;
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
