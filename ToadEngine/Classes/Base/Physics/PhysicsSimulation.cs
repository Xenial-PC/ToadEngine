using BepuPhysics.CollisionDetection;
using BepuPhysics;
using BepuUtilities.Memory;
using BepuUtilities;

namespace ToadEngine.Classes.Base.Physics;

public class PhysicsComponent
{
    public BodyHandle BodyHandle;
    public bool IsDynamic;
}

public class PhysicsSimulation : IDisposable
{
    public bool IsPhysicsPaused, IsSetup;
    public Simulation Simulation { get; set; } = null!;
    public BufferPool BufferPool { get; set; } = null!;
    public ColliderManager ColliderManager { get; set; } = null!;

    public ThreadDispatcher ThreadDispatcher = null!;

    public T NarrowPhaseAs<T>() where T : struct => (T)_narrowPhase;
    public T PoseIntegratorAs<T>() where T : struct => (T)_poseIntegrator;

    private INarrowPhaseCallbacks _narrowPhase = null!;
    private IPoseIntegratorCallbacks _poseIntegrator = null!;

    public SolveDescription SolveDescription { get; set; } = new(8, 1);

    public void Setup()
    {
        if (IsSetup) return;

        ThreadDispatcher = new ThreadDispatcher(int.Max(1,
            Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1));

        var narrowPhase = new DefaultCallBacks.NarrowPhaseCallbacks();
        _narrowPhase = narrowPhase;

        var pose = new DefaultCallBacks.PoseIntegratorCallbacks() { Gravity = new System.Numerics.Vector3(0, -10, 0) };
        _poseIntegrator = pose;

        BufferPool = new BufferPool();
        Simulation = Simulation.Create(BufferPool, narrowPhase, pose, SolveDescription);
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
        _narrowPhase = narrowPhase;

        var pose = new TPose();
        _poseIntegrator = pose;

        BufferPool = new BufferPool();
        Simulation = Simulation.Create(BufferPool, narrowPhase, pose, SolveDescription);
        ColliderManager = new ColliderManager(Simulation, BufferPool);
        IsSetup = true;
    }

    public void Step(float deltaTime)
    {
        Simulation.Timestep(deltaTime);
        TriggerManager.EndFrame();
    }

    public void Dispose()
    {
        TriggerRegistry.Triggers.Clear();
        ThreadDispatcher.Dispose();
        Simulation.Dispose();
        ((IDisposable)BufferPool).Dispose();
    }
}
