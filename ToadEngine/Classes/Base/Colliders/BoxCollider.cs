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
                Collider = GetCurrentScene().PhysicsManager.CreateTriggerBox(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Kinematic:
                Collider = GetCurrentScene().PhysicsManager.CreateKinematicBox(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Dynamic:
                Collider = GetCurrentScene().PhysicsManager.CreateBox(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Static:
                var h = GetCurrentScene().PhysicsManager.CreateStaticBox(
                    (Vector3)GameObject.Transform.Position,
                    (Vector3)Size);

                Handle = h.Value;
                BodyToGameObject[Handle] = GameObject;
                break;
        }
    }

    public override void Update(float deltaTime)
    {
        if (_lastSize == Size) return;
        _lastSize = Size;

        ResizeBox();
    }

    private void ResizeBox()
    {
        var shape = new Box(Size.X, Size.Y, Size.Z);
        Resize(shape);
    }
}
