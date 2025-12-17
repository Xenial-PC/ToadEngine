using BepuPhysics.Collidables;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Colliders;

public class BoxCollider : Collider
{
    public OpenTK.Mathematics.Vector3 Size = OpenTK.Mathematics.Vector3.Zero;
    
    public void Awake()
    {
        if (Size == OpenTK.Mathematics.Vector3.Zero) 
            Size = GameObject.Transform.LocalScale;

        switch (Type)
        {
            case ColliderType.Trigger:
                BHandle = Physics.ColliderManager.CreateTrigger.Box(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size);

                Handle = BHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Kinematic:
                BHandle = Physics.ColliderManager.CreateKinematic.Box(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size);

                Handle = BHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Dynamic:
                BHandle = Physics.ColliderManager.CreateDynamic.Box(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size, Mass);

                Handle = BHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Static:
                SHandle = Physics.ColliderManager.CreateStatic.Box(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size);

                Handle = SHandle.Value;
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
