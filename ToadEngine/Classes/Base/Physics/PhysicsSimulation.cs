using BepuPhysics.CollisionDetection;
using BepuPhysics;
using BepuPhysics.Constraints;
using BepuUtilities.Memory;
using BepuUtilities;
using Vector3 = System.Numerics.Vector3;
using ToadEngine.Classes.Base.Physics.Managers;

namespace ToadEngine.Classes.Base.Physics;

public class PhysicsSettings
{
    public Vector3 Gravity = new(0, -10, 0);
    public SolveDescription SolveDescription = new(8, 1);
    public SpringSettings SpringSettings = new(30, 1);

    public AngularIntegrationMode AngularIntegrationMode = AngularIntegrationMode.Nonconserving;
    public bool AllowSubstepsForUnconstrainedBodies = false;
    public bool IntegrateVelocityForKinematics = false;

    public float Restitution = 8f;
    public float Friction = 1f;
}

public class PhysicsSimulation : IDisposable
{
    public bool IsPhysicsPaused, IsSetup;

    public Simulation Simulation { get; set; } = null!;
    public BufferPool BufferPool { get; set; } = null!;
    public ColliderManager ColliderManager { get; set; } = null!;
    public PhysicsSettings Settings { get; } = new();
    public PhysicsActions Actions { get; } = new();

    public ThreadDispatcher ThreadDispatcher = null!;

    public PhysicsMaterial RegisterMaterial(PhysicsMaterial material) => PhysicsMaterialRegistry.Register(material);
    public PhysicsMaterial BindMaterial(int handle, PhysicsMaterial material) => PhysicsMaterialRegistry.Register(handle, material);

    public PhysicsMaterial? GetMaterial(string name) => PhysicsMaterialRegistry.Get(name);
    public PhysicsMaterial? GetBoundMaterial(int index) => PhysicsMaterialRegistry.Get(index);

    public void SetMaterial(int handle, PhysicsMaterial material) => PhysicsMaterialRegistry.Set(handle, RegisterMaterial(material));

    public void Setup()
    {
        if (IsSetup) return;

        ThreadDispatcher = new ThreadDispatcher(int.Max(1,
            Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1));

        var narrowPhase = new PhysicsCallBacks.NarrowPhaseCallbacks();
        var pose = new PhysicsCallBacks.PoseIntegratorCallbacks();
        
        BufferPool = new BufferPool();
        Simulation = Simulation.Create(BufferPool, narrowPhase, pose, Settings.SolveDescription);
        ColliderManager = new ColliderManager(Simulation, BufferPool);
        IsSetup = true;
    }

    public void Setup<TNarrow, TPose>()
        where TNarrow : struct, INarrowPhaseCallbacks
        where TPose : struct, IPoseIntegratorCallbacks
    {
        if (IsSetup) return;

        ThreadDispatcher = new ThreadDispatcher(int.Max(1,
            Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1));

        var narrowPhase = new TNarrow();
        var pose = new TPose();
        
        BufferPool = new BufferPool();
        Simulation = Simulation.Create(BufferPool, narrowPhase, pose, Settings.SolveDescription);
        ColliderManager = new ColliderManager(Simulation, BufferPool);
        IsSetup = true;
    }

    public void Step(float deltaTime)
    {
        Actions.OnPreStep?.Invoke(deltaTime);
        Simulation.Timestep(deltaTime);
        Actions.OnPostStep?.Invoke(deltaTime);

        TriggerManager.EndFrame();
    }

    public void Dispose()
    {
        TriggerManager.Reset();
        ThreadDispatcher.Dispose();
        Simulation.Dispose();
        ((IDisposable)BufferPool).Dispose();
    }
}
