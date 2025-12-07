using BepuPhysics.Collidables;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Colliders;

public class BoxCollider : BaseCollider
{
    public OpenTK.Mathematics.Vector3 Size = OpenTK.Mathematics.Vector3.Zero;
    
    public void Start()
    {
        if (Size == OpenTK.Mathematics.Vector3.Zero) 
            Size = GameObject.Transform.LocalScale;

        switch (Type)
        {
            case ColliderType.Trigger:
                Collider = Physics.ColliderManager.CreateTrigger.Box(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Kinematic:
                Collider = Physics.ColliderManager.CreateKinematic.Box(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Dynamic:
                Collider = Physics.ColliderManager.CreateDynamic.Box(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Static:
                SCollider = Physics.ColliderManager.CreateStatic.Box(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size);

                Handle = SCollider.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
        }
    }

    public void UpdateBoundingBox()
    {
        var shape = new Box(Size.X, Size.Y, Size.Z);
        Resize(shape);
    }
}
