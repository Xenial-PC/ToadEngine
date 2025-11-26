using BepuPhysics;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Colliders;

public class BoxCollider : BaseCollider
{
    public OpenTK.Mathematics.Vector3 Size = OpenTK.Mathematics.Vector3.Zero;

    public override void Setup()
    {
        if (Size == OpenTK.Mathematics.Vector3.Zero) 
            Size = GameObject.Transform.LocalScale;
        
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
}
