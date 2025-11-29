using BepuPhysics;
using BepuPhysics.Collidables;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Colliders;

public class BoxCollider : BaseCollider
{
    public OpenTK.Mathematics.Vector3 Size = OpenTK.Mathematics.Vector3.Zero;
    private OpenTK.Mathematics.Vector3 _lastSize = OpenTK.Mathematics.Vector3.Zero;

    public override void Setup()
    {
        if (Size == OpenTK.Mathematics.Vector3.Zero) 
            Size = GameObject.Transform.LocalScale;

        _lastSize = Size;

        switch (Type)
        {
            case ColliderType.Trigger:
                Collider = Scene.PhysicsManager.CreateTriggerBox(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Kinematic:
                Collider = Scene.PhysicsManager.CreateKinematicBox(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Dynamic:
                Collider = Scene.PhysicsManager.CreateBox(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Static:
                SCollider = Scene.PhysicsManager.CreateStaticBox(
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
