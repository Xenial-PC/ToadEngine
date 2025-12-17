using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Colliders;

public class MeshCollider : Collider
{
    public OpenTK.Mathematics.Vector3 Size = OpenTK.Mathematics.Vector3.Zero;
    public Buffer<Triangle> Triangles;

    public void Awake()
    {
        if (Size == OpenTK.Mathematics.Vector3.Zero)
            Size = GameObject.Transform.LocalScale;

        switch (Type)
        {
            case ColliderType.Trigger:
                BHandle = Physics.ColliderManager.CreateTrigger.Mesh(
                    (Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)Size);

                Handle = BHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Kinematic:
                BHandle = Physics.ColliderManager.CreateKinematic.Mesh((Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)Size);

                Handle = BHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Dynamic:
                BHandle = Physics.ColliderManager.CreateDynamic.Mesh((Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)Size, Mass);

                Handle = BHandle.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
            case ColliderType.Static:
            {
                SHandle = Physics.ColliderManager.CreateStatic.Mesh((Vector3)GameObject.Transform.Position,
                    Triangles, (Vector3)Size);

                Handle = SHandle.Value;
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
