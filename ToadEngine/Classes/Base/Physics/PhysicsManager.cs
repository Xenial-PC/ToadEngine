using System.Numerics;
using System.Runtime.CompilerServices;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using Quaternion = System.Numerics.Quaternion;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Physics;

public class PhysicsComponent
{
    public BodyHandle BodyHandle;
    public bool IsDynamic;
}

public class TriggerRegistry
{
    public static readonly HashSet<int> Triggers = new();
    public static bool IsTrigger(int handle) => Triggers.Contains(handle);
}

public static class Trigger
{
    public static readonly HashSet<(int, int)> ActiveOverlaps = new();
    private static readonly HashSet<(int, int)> ThisFrameOverlaps = new();

    public static Action<int, int>? OnEnter;
    public static Action<int, int>? OnExit;

    public static void RegisterOverlap(int a, int b)
    {
        if (a > b) (a, b) = (b, a);
        ThisFrameOverlaps.Add((a, b));

        if (ActiveOverlaps.Add((a, b)))
            OnEnter?.Invoke(a, b);
    }

    public static void EndFrame()
    {
        foreach (var pair in ActiveOverlaps.Except(ThisFrameOverlaps).ToList())
        {
            OnExit?.Invoke(pair.Item1, pair.Item2);
            ActiveOverlaps.Remove(pair);
        }

        ThisFrameOverlaps.Clear();
    }

    public static bool IsActive(int a, int b)
        => ActiveOverlaps.Contains((a, b));
}

public class PhysicsManager
{
    public bool IsPhysicsPaused;
    public Simulation Simulation { get; private set; }
    public BufferPool BufferPool { get; private set; }

    public ThreadDispatcher ThreadDispatcher = new ThreadDispatcher(int.Max(1,
        Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1));

    public PhysicsManager()
    {
        BufferPool = new BufferPool();
        Simulation = Simulation.Create(
            BufferPool, new NarrowPhaseCallbacks(), new PoseIntegratorCallbacks(new System.Numerics.Vector3(0, -10, 0)), 
            new SolveDescription(8, 1));
    }

    public void Step(float deltaTime)
    {
        Simulation.Timestep(deltaTime);
        Trigger.EndFrame();
    }

    public BodyHandle CreateBox(Vector3 pos, Vector3 size, float mass = 1f)
    {
        var shape = new Box(size.X, size.Y, size.Z);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = BodyDescription.CreateDynamic(
            new RigidPose(pos, Quaternion.Identity),
            shape.ComputeInertia(mass),
            new CollidableDescription(shapeIndex, 0.1f),
            new BodyActivityDescription(0.01f)
        );
        return Simulation.Bodies.Add(bodyDesc);
    }

    public StaticHandle CreateStaticBox(Vector3 pos, Vector3 size)
    {
        var shape = new Box(size.X, size.Y, size.Z);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = new StaticDescription(
            new RigidPose(pos, Quaternion.Identity),
            shapeIndex
        );
        
        return Simulation.Statics.Add(bodyDesc);
    }

    public BodyHandle CreateKinematicBox(Vector3 pos, Vector3 size)
    {
        var shape = new Box(size.X, size.Y, size.Z);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = BodyDescription.CreateKinematic(
            new RigidPose(pos),
            new CollidableDescription(shapeIndex, 0.1f),
            new BodyActivityDescription(0.01f));

        return Simulation.Bodies.Add(bodyDesc);
    }

    public BodyHandle CreateTriggerBox(Vector3 pos, Vector3 size)
    {
        var shape = new Box(size.X, size.Y, size.Z);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = BodyDescription.CreateKinematic(
            new RigidPose(pos),
            new CollidableDescription(shapeIndex, 0.1f),
            new BodyActivityDescription(0.01f));

        var handle = Simulation.Bodies.Add(bodyDesc);
        TriggerRegistry.Triggers.Add(handle.Value);
        return handle;
    }

    public BodyHandle CreateSphere(Vector3 pos, float radius, float mass = 1f)
    {
        var shape = new Sphere(radius);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = BodyDescription.CreateDynamic(
            new RigidPose(pos, Quaternion.Identity),
            shape.ComputeInertia(mass),
            new CollidableDescription(shapeIndex, 0.1f),
            new BodyActivityDescription(0.01f)
        );
        return Simulation.Bodies.Add(bodyDesc);
    }

    public StaticHandle CreateStaticSphere(Vector3 pos, float radius)
    {
        var shape = new Sphere(radius);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = new StaticDescription(
            new RigidPose(pos, Quaternion.Identity),
            shapeIndex
        );
        return Simulation.Statics.Add(bodyDesc);
    }

    public BodyHandle CreateKinematicSphere(Vector3 pos, float radius)
    {
        var shape = new Sphere(radius);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = BodyDescription.CreateKinematic(
            new RigidPose(pos),
            new CollidableDescription(shapeIndex, 0.1f),
            new BodyActivityDescription(0.01f));

        return Simulation.Bodies.Add(bodyDesc);
    }

    public BodyHandle CreateTriggerSphere(Vector3 pos, float radius)
    {
        var shape = new Sphere(radius);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = BodyDescription.CreateKinematic(
            new RigidPose(pos),
            new CollidableDescription(shapeIndex, 0.1f),
            new BodyActivityDescription(0.01f));

        var handle = Simulation.Bodies.Add(bodyDesc);
        TriggerRegistry.Triggers.Add(handle.Value);

        return handle;
    }

    public BodyHandle CreateCapsule(Vector3 pos, Vector2 radLength, float mass = 1f)
    {
        var shape = new Capsule(radLength.X, radLength.Y);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = BodyDescription.CreateDynamic(
            new RigidPose(pos, Quaternion.Identity),
            shape.ComputeInertia(mass), 
            new CollidableDescription(shapeIndex, 0.1f),
            new BodyActivityDescription(0.01f)
        );
        return Simulation.Bodies.Add(bodyDesc);
    }

    public StaticHandle CreateStaticCapsule(Vector3 pos, Vector2 radLength)
    {
        var shape = new Capsule(radLength.X, radLength.Y);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = new StaticDescription(
            new RigidPose(pos, Quaternion.Identity),
            shapeIndex
        );
        return Simulation.Statics.Add(bodyDesc);
    }

    public BodyHandle CreateKinematicCapsule(Vector3 pos, Vector2 radLength)
    {
        var shape = new Capsule(radLength.X, radLength.Y);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = BodyDescription.CreateKinematic(
            new RigidPose(pos),
            new CollidableDescription(shapeIndex, 0.1f),
            new BodyActivityDescription(0.01f));

        return Simulation.Bodies.Add(bodyDesc);
    }

    public BodyHandle CreateTriggerCapsule(Vector3 pos, Vector2 radLength)
    {
        var shape = new Capsule(radLength.X, radLength.Y);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = BodyDescription.CreateKinematic(
            new RigidPose(pos),
            new CollidableDescription(shapeIndex, 0.1f),
            new BodyActivityDescription(0.01f));

        var handle = Simulation.Bodies.Add(bodyDesc);
        TriggerRegistry.Triggers.Add(handle.Value);

        return handle;
    }

    public BodyHandle CreateCylinder(Vector3 pos, Vector2 radLength, float mass = 1f)
    {
        var shape = new Cylinder(radLength.X, radLength.Y);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = BodyDescription.CreateDynamic(
            new RigidPose(pos, Quaternion.Identity),
            shape.ComputeInertia(mass),
            new CollidableDescription(shapeIndex, 0.1f),
            new BodyActivityDescription(0.01f)
        );
        return Simulation.Bodies.Add(bodyDesc);
    }

    public StaticHandle CreateStaticCylinder(Vector3 pos, Vector2 radLength)
    {
        var shape = new Cylinder(radLength.X, radLength.Y);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = new StaticDescription(
            new RigidPose(pos, Quaternion.Identity),
            shapeIndex
        );
        return Simulation.Statics.Add(bodyDesc);
    }

    public BodyHandle CreateKinematicCylinder(Vector3 pos, Vector2 radLength)
    {
        var shape = new Cylinder(radLength.X, radLength.Y);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = BodyDescription.CreateKinematic(
            new RigidPose(pos),
            new CollidableDescription(shapeIndex, 0.1f),
            new BodyActivityDescription(0.01f));

        return Simulation.Bodies.Add(bodyDesc);
    }

    public BodyHandle CreateTriggerCylinder(Vector3 pos, Vector2 radLength)
    {
        var shape = new Cylinder(radLength.X, radLength.Y);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = BodyDescription.CreateKinematic(
            new RigidPose(pos),
            new CollidableDescription(shapeIndex, 0.1f),
            new BodyActivityDescription(0.01f));

        var handle = Simulation.Bodies.Add(bodyDesc);
        TriggerRegistry.Triggers.Add(handle.Value);

        return handle;
    }

    public BodyHandle CreateMesh(Vector3 pos, Buffer<Triangle> triangles, Vector3 scale, float mass = 1f)
    {
        var shape = new Mesh(triangles, scale, BufferPool);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = BodyDescription.CreateDynamic(
            new RigidPose(pos, Quaternion.Identity),
            shape.ComputeClosedInertia(mass),
            new CollidableDescription(shapeIndex, 0.1f),
            new BodyActivityDescription(0.01f)
        );
        return Simulation.Bodies.Add(bodyDesc);
    }

    public StaticHandle CreateStaticMesh(Vector3 pos, Buffer<Triangle> triangles, Vector3 scale)
    {
        var shape = new Mesh(triangles, scale, BufferPool);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = new StaticDescription(
            new RigidPose(pos, Quaternion.Identity),
            shapeIndex
        );
        return Simulation.Statics.Add(bodyDesc);
    }

    public BodyHandle CreateKinematicMesh(Vector3 pos, Buffer<Triangle> triangles, Vector3 scale)
    {
        var shape = new Mesh(triangles, scale, BufferPool);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = BodyDescription.CreateKinematic(
            new RigidPose(pos),
            new CollidableDescription(shapeIndex, 0.1f),
            new BodyActivityDescription(0.01f));

        return Simulation.Bodies.Add(bodyDesc);
    }

    public BodyHandle CreateTriggerMesh(Vector3 pos, Buffer<Triangle> triangles, Vector3 scale)
    {
        var shape = new Mesh(triangles, scale, BufferPool);
        var shapeIndex = Simulation.Shapes.Add(shape);
        var bodyDesc = BodyDescription.CreateKinematic(
            new RigidPose(pos),
            new CollidableDescription(shapeIndex, 0.1f),
            new BodyActivityDescription(0.01f));

        var handle = Simulation.Bodies.Add(bodyDesc);
        TriggerRegistry.Triggers.Add(handle.Value);

        return handle;
    }

    struct NarrowPhaseCallbacks(float maximumRecoveryVelocity = 8f, float frictionCoefficient = 1f) : INarrowPhaseCallbacks
    {
        public float MaximumRecoveryVelocity = maximumRecoveryVelocity, FrictionCoefficient = frictionCoefficient;

        public void Initialize(Simulation simulation)
        {
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b,
            ref float speculativeMargin)
        {
            return (a.Mobility == CollidableMobility.Dynamic || b.Mobility == CollidableMobility.Dynamic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
        {
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ConfigureContactManifold<TManifold>(int workerIndex, CollidablePair pair, ref TManifold manifold,
            out PairMaterialProperties pairMaterial) where TManifold : unmanaged, IContactManifold<TManifold>
        {
            pairMaterial.FrictionCoefficient = FrictionCoefficient;
            pairMaterial.MaximumRecoveryVelocity = MaximumRecoveryVelocity;
            pairMaterial.SpringSettings = new SpringSettings(30, 1);

            if (IsTrigger(pair.A) || IsTrigger(pair.B))
            {
                Trigger.RegisterOverlap(pair.A.RawHandleValue, pair.B.RawHandleValue);
                return false;
            }
            
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB,
            ref ConvexContactManifold manifold)
        {
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsTrigger(CollidableReference collidable)
        {
            return collidable.Mobility != CollidableMobility.Static &&
                   TriggerRegistry.IsTrigger(collidable.RawHandleValue);
        }

        public void Dispose()
        {

        }
    }

    public struct PoseIntegratorCallbacks(System.Numerics.Vector3 gravity) : IPoseIntegratorCallbacks
    {
        public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;
        public bool AllowSubstepsForUnconstrainedBodies => false;
        public bool IntegrateVelocityForKinematics => false;

        public System.Numerics.Vector3 Gravity = gravity;
        private Vector3Wide _gravityWideDt;

        public void Initialize(Simulation simulation)
        {
            
        }

        public void PrepareForIntegration(float dt)
        {
            _gravityWideDt = Vector3Wide.Broadcast(Gravity * dt);
        }

        public void IntegrateVelocity(Vector<int> bodyIndices, Vector3Wide position, QuaternionWide orientation,
            BodyInertiaWide localInertia, Vector<int> integrationMask, int workerIndex, Vector<float> dt, ref BodyVelocityWide velocity)
        {
            velocity.Linear += _gravityWideDt;
        }
    }
}
