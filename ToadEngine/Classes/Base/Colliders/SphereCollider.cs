using BepuPhysics.Collidables;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Colliders;

public class SphereCollider : BaseCollider
{
    public float Radius;

    public void Awake()
    {
        switch (Type)
        {
            case ColliderType.Trigger:
                Collider = Physics.ColliderManager.CreateTrigger.Sphere((Vector3)GameObject.Transform.Position, Radius);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Kinematic:
                Collider = Physics.ColliderManager.CreateKinematic.Sphere((Vector3)GameObject.Transform.Position, Radius);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Dynamic:
                Collider = Physics.ColliderManager.CreateDynamic.Sphere((Vector3)GameObject.Transform.Position, Radius, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Static:
                SCollider = Physics.ColliderManager.CreateStatic.Sphere((Vector3)GameObject.Transform.Position, Radius);

                Handle = SCollider.Value;
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
