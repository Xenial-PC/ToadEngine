using BepuPhysics.CollisionDetection;
using BepuPhysics;
using BepuPhysics.Constraints;
using BepuUtilities.Memory;
using BepuUtilities;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Physics;

public class PhysicsSimulation : IDisposable
{
    public bool IsPhysicsPaused, IsSetup;

    public Simulation Simulation { get; set; } = null!;
    public BufferPool BufferPool { get; set; } = null!;
    public ColliderManager ColliderManager { get; set; } = null!;

    /// <summary>
    /// Default Callback field for setting gravity
    /// </summary>
    public Vector3 Gravity = new(0, -10, 0);

    public float MaximumRecoveryVelocity = 8f, FrictionCoefficient = 1f;
    public SpringSettings SpringSettings = new(30, 1);

    public ThreadDispatcher ThreadDispatcher = null!;

    public SolveDescription SolveDescription { get; set; } = new(8, 1);

    public void Setup()
    {
        if (IsSetup) return;

        ThreadDispatcher = new ThreadDispatcher(int.Max(1,
            Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1));

        var narrowPhase = new DefaultCallBacks.NarrowPhaseCallbacks();
        var pose = new DefaultCallBacks.PoseIntegratorCallbacks();
        
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
        var pose = new TPose();
        
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
