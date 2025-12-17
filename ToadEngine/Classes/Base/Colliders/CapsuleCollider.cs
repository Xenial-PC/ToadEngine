using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using BepuPhysics.Collidables;

namespace ToadEngine.Classes.Base.Colliders;

public class CapsuleCollider : Collider
{
    public Vector2 Size = Vector2.Zero;
    public float Radius;

    public void Awake()
    {
        if (Size == Vector2.Zero)
            Size = new Vector2(Radius, GameObject.Transform.LocalScale.Y);

        switch (Type)
        {
            case ColliderType.Trigger:
                BHandle = Physics.ColliderManager.CreateTrigger.Capsule((Vector3)GameObject.Transform.Position, Size);

                Handle = BHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Kinematic:
                BHandle = Physics.ColliderManager.CreateKinematic.Capsule((Vector3)GameObject.Transform.Position, Size);

                Handle = BHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Dynamic:
                BHandle = Physics.ColliderManager.CreateDynamic.Capsule((Vector3)GameObject.Transform.Position, Size, Mass);

                Handle = BHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Static:
                SHandle = Physics.ColliderManager.CreateStatic.Capsule((Vector3)GameObject.Transform.Position, Size);

                Handle = SHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
        }
    }

    public void UpdateBoundingBox()
    {
        var shape = new Capsule(Radius, Size.Y);
        Resize(shape);
    }
}
