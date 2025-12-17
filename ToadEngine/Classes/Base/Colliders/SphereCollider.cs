using BepuPhysics.Collidables;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Colliders;

public class SphereCollider : Collider
{
    public float Radius;

    public void Awake()
    {
        switch (Type)
        {
            case ColliderType.Trigger:
                BHandle = Physics.ColliderManager.CreateTrigger.Sphere((Vector3)GameObject.Transform.Position, Radius);

                Handle = BHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Kinematic:
                BHandle = Physics.ColliderManager.CreateKinematic.Sphere((Vector3)GameObject.Transform.Position, Radius);

                Handle = BHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Dynamic:
                BHandle = Physics.ColliderManager.CreateDynamic.Sphere((Vector3)GameObject.Transform.Position, Radius, Mass);

                Handle = BHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Static:
                SHandle = Physics.ColliderManager.CreateStatic.Sphere((Vector3)GameObject.Transform.Position, Radius);

                Handle = SHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
        }
    }

    public void UpdateBoundingBox()
    {
        var shape = new Sphere(Radius);
        Resize(shape);
    }
}
