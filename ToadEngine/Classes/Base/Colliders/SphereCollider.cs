using BepuPhysics;
using BepuPhysics.Collidables;
using System.Drawing;
using ToadEngine.Classes.Extensions;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Colliders;

public class SphereCollider : BaseCollider
{
    public float Radius;

    private float _lastRadius;

    public override void Setup()
    {
        _lastRadius = Radius;

        switch (Type)
        {
            case ColliderType.Trigger:
                Collider = GetCurrentScene().PhysicsManager.CreateTriggerSphere((Vector3)GameObject.Transform.Position, Radius);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Kinematic:
                Collider = GetCurrentScene().PhysicsManager.CreateKinematicSphere((Vector3)GameObject.Transform.Position, Radius);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Dynamic:
                Collider = GetCurrentScene().PhysicsManager.CreateSphere((Vector3)GameObject.Transform.Position, Radius, Mass);

                Handle = Collider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
            case ColliderType.Static:
                SCollider = GetCurrentScene().PhysicsManager.CreateStaticSphere((Vector3)GameObject.Transform.Position, Radius);

                Handle = SCollider.Value;
                BodyToGameObject[Handle] = GameObject;
                return;
        }
    }

    public override void Update(float deltaTime)
    {
        if (_lastRadius == Radius) return;
        _lastRadius = Radius;

        ResizeSphere();
    }

    private void ResizeSphere()
    {
        var shape = new Sphere(Radius);
        Resize(shape);
    }
}
