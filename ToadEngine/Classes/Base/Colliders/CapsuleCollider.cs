using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using BepuPhysics.Collidables;

namespace ToadEngine.Classes.Base.Colliders;

public class CapsuleCollider : BaseCollider
{
    public Vector2 Size = Vector2.Zero;
    public float Radius;

    public void Start()
    {
        if (Size == Vector2.Zero)
            Size = new Vector2(Radius, GameObject.Transform.LocalScale.Y);

        switch (Type)
        {
            case ColliderType.Trigger:
                Collider = Physics.ColliderManager.CreateTrigger.Capsule((Vector3)GameObject.Transform.Position, Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Kinematic:
                Collider = Physics.ColliderManager.CreateKinematic.Capsule((Vector3)GameObject.Transform.Position, Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Dynamic:
                Collider = Physics.ColliderManager.CreateDynamic.Capsule((Vector3)GameObject.Transform.Position, Size, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Static:
                SCollider = Physics.ColliderManager.CreateStatic.Capsule((Vector3)GameObject.Transform.Position, Size);

                Handle = SCollider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
        }
    }

    public void UpdateBoundingBox()
    {
        var shape = new Capsule(Radius, Size.Y);
        Resize(shape);
    }
}
