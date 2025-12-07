using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Colliders;

public class MeshCollider : BaseCollider
{
    public OpenTK.Mathematics.Vector3 Size = OpenTK.Mathematics.Vector3.Zero;
    public Buffer<Triangle> Triangles;

    public override void OnStart()
    {
        if (Size == OpenTK.Mathematics.Vector3.Zero)
            Size = GameObject.Transform.LocalScale;

        switch (Type)
        {
            case ColliderType.Trigger:
                Collider = Physics.ColliderManager.CreateTrigger.Mesh(
                    (Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Kinematic:
                Collider = Physics.ColliderManager.CreateKinematic.Mesh((Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Dynamic:
                Collider = Physics.ColliderManager.CreateDynamic.Mesh((Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)Size, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Static:
            {
                SCollider = Physics.ColliderManager.CreateStatic.Mesh((Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)Size);

                Handle = SCollider.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            }
        }
    }

    public void UpdateBoundingBox()
    {
        var shape = new Mesh(Triangles, (Vector3)Size, Physics.BufferPool);
        Resize(shape);
    }
}
