using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Trees;
using BepuUtilities;
using BepuUtilities.Collections;
using BepuUtilities.Memory;
using PInvoke;
using ToadEngine.Classes.Base.Physics;
using Buffer = OpenTK.Graphics.OpenGL4.Buffer;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace ToadEngine.Classes.Base.Raycasting;

public class RaycastManager
{
    private Buffer<RayJob> _jobs;
    private QuickList<Ray> _rays;
    private Buffer<RayHit> _results;

    private readonly BufferPool _pool;
    private readonly IntersectionAlgorithm _intersection = new();

    public RaycastManager(BufferPool pool, int initCapacity = 1024)
    {
        _pool = pool;
        _rays = new QuickList<Ray>(1024, pool);
        _pool.Take(initCapacity, out _jobs);
    }

    public unsafe RayHit CastSingleRay(Ray ray)
    {
        _pool.Take(1, out Buffer<RayHit> hits);
        hits[0] = new RayHit { Distance = float.MaxValue, IsHit = false };

        var intersectionCount = 0;
        var hitHandler = new HitHandler
        {
            Hits = hits,
            IntersectionCount = &intersectionCount
        };

        GetCurrentSimulation().RayCast(ray.Origin, ray.Direction, ray.MaximumT, ref hitHandler);
        return hitHandler.Hits[0];
    }

    public RayHit SendRay(Vector3 origin, Vector3 direction, float maxT = 1000f)
    {
        if (direction == Vector3.Zero)
            throw new ArgumentException("Direction cannot be zero", nameof(direction));

        var ray = new Ray
        {
            Origin = (System.Numerics.Vector3)origin,
            Direction = (System.Numerics.Vector3)Vector3.Normalize(direction),
            MaximumT = maxT
        };

        return CastSingleRay(ray);
    }

    public RayHit[] SendRays(List<Ray>? rays)
    {
        if (rays == null || rays.Count == 0)
            return [];

        var count = rays.Count;

        EnsureResultsCapacity(count);

        var results = _results;
        for (var i = 0; i < count; i++)
            results[i] = new RayHit { Distance = float.MaxValue, IsHit = false };

        _intersection.Update("BatchedRays", BatchedWorker, _pool, count);
        _intersection.Results = results;

        _rays = new QuickList<Ray>(count, _pool);
        for (var i = 0; i < count; i++)
            _rays.AllocateUnsafely() = rays[i];

        var jobCount = Math.Max(1, GetCurrentDispatcher().ThreadCount);
        _pool.Take(jobCount, out _jobs);

        var raysPerJob = count / jobCount;
        var remainder = count % jobCount;
        var start = 0;
        for (var i = 0; i < jobCount; i++)
        {
            var end = start + raysPerJob + (i < remainder ? 1 : 0);
            _jobs[i] = new RayJob { Start = start, End = end };
            start = end;
        }

        _intersection.Execute(ref _rays, GetCurrentDispatcher());

        var managedResults = new RayHit[count];
        for (var i = 0; i < count; i++)
            managedResults[i] = results[i];

        _rays.Dispose(_pool);
        _pool.Return(ref _jobs);

        return managedResults;
    }

    public void EnsureResultsCapacity(int count)
    {
        if (_results.Length < count)
        {
            if (_results.Length > 0) _pool.Return(ref _results);
            _pool.Take(count, out _results);
        }

        for (var i = 0; i < count; i++)
            _results[i] = new RayHit()
            {
                Distance = float.MaxValue,
                IsHit = false
            };
    }

    private ThreadDispatcher GetCurrentDispatcher()
    {
        return RenderObject.GetCurrentScene().PhysicsManager.ThreadDispatcher;
    }

    private Simulation GetCurrentSimulation()
    {
        return RenderObject.GetCurrentScene().PhysicsManager.Simulation;
    }

    private unsafe int BatchedWorker(int workerIndex, IntersectionAlgorithm algorithm)
    {
        var intersectionCount = 0;
        var hitHandler = new HitHandler { Hits = algorithm.Results, IntersectionCount = &intersectionCount };
        var batcher = new SimulationRayBatcher<HitHandler>(GetCurrentDispatcher().GetThreadMemoryPool(workerIndex),
            GetCurrentSimulation(), hitHandler, 2048);

        int claimedIndex;
        while ((claimedIndex = Interlocked.Increment(ref algorithm.JobIndex)) < _jobs.Length)
        {
            ref var job = ref _jobs[claimedIndex];
            for (var i = job.Start; i < job.End; ++i)
            {
                ref var ray = ref _rays[i];
                batcher.Add(ref ray.Origin, ref ray.Direction, ray.MaximumT, i);
            }
        }

        batcher.Flush();
        batcher.Dispose();
        return intersectionCount;
    }

    private unsafe int UnBatchWorker(int workerIndex, IntersectionAlgorithm algorithm)
    {
        var intersectionCount = 0;
        var hitHandler = new HitHandler { Hits = algorithm.Results, IntersectionCount = &intersectionCount };
        var claimedIndex = 0;
        var pool = GetCurrentDispatcher().GetThreadMemoryPool(workerIndex);
        while ((claimedIndex = Interlocked.Increment(ref algorithm.JobIndex)) < _jobs.Length)
        {
            ref var job = ref _jobs[claimedIndex];
            for (var i = job.Start; i < job.End; ++i)
            {
                ref var ray = ref _rays[i];
                GetCurrentSimulation().RayCast((System.Numerics.Vector3)ray.Origin, (System.Numerics.Vector3)ray.Direction, ray.MaximumT, ref hitHandler, i);
            }
        }
        return intersectionCount;
    }

    public struct Ray
    {
        public System.Numerics.Vector3 Origin;
        public float MaximumT;
        public System.Numerics.Vector3 Direction;
    }

    public struct RayHit
    {
        public System.Numerics.Vector3 Normal;
        public float Distance;
        public CollidableReference Collidabe;
        public bool IsHit;
    }

    public struct RayJob
    {
        public int Start;
        public int End;
    }

    public unsafe class IntersectionAlgorithm
    {
        public string Name;
        public int IntersectionCount;
        public Buffer<RayHit> Results;
        public TimingsRingBuffer Timings;

        public Func<int, IntersectionAlgorithm, int> Worker;
        public Action<int> InternalWorker;
        public int JobIndex;

        public void Update(string name, Func<int, IntersectionAlgorithm, int> worker, BufferPool pool,
            int largestRayCount, int timingSampleCount = 16)
        {
            Name = name;
            Timings = new TimingsRingBuffer(timingSampleCount, pool);
            Worker = worker;
            InternalWorker = ExecuteWorker;
            pool.Take(largestRayCount, out Results);
        }

        private void ExecuteWorker(int workerIndex)
        {
            var intersectionCount = Worker(workerIndex, this);
            Interlocked.Add(ref IntersectionCount, intersectionCount);
        }

        public void Execute(ref QuickList<Ray> rays, IThreadDispatcher? dispatcher)
        {
            for (var i = 0; i < rays.Count; ++i)
            {
                Results[i].Distance = float.MaxValue;
                Results[i].IsHit = false;
            }

            JobIndex = -1;
            IntersectionCount = 0;

            var start = Stopwatch.GetTimestamp();
            if (dispatcher != null) dispatcher.DispatchWorkers(InternalWorker);
            else InternalWorker(0);
            var stop = Stopwatch.GetTimestamp();
            Timings.Add((stop - start) / (double)Stopwatch.Frequency);
        }
    }

    public unsafe struct HitHandler : IRayHitHandler
    {
        public Buffer<RayHit> Hits;
        public int* IntersectionCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowTest(CollidableReference collidable)
        {
            return !TriggerRegistry.IsTrigger(collidable.RawHandleValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowTest(CollidableReference collidable, int childIndex)
        {
            return !TriggerRegistry.IsTrigger(collidable.RawHandleValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnRayHit(in RayData ray, ref float maximumT, float t, in System.Numerics.Vector3 normal, CollidableReference collidable,
            int childIndex)
        {
            maximumT = t;
            ref var hit = ref Hits[ray.Id];

            if (t > hit.Distance) return;
            if (hit.Distance is float.MaxValue) ++*IntersectionCount;

            hit.Normal = normal;
            hit.Distance = t;
            hit.Collidabe = collidable;
            hit.IsHit = true;
        }
    }

    public interface IDataSeries
    {
        public int Start { get; }
        public int End { get; }
        public double this[int index] { get; }
    }

    public class TimingsRingBuffer : IDataSeries, IDisposable
    {
        private QuickQueue<double> _queue;
        private readonly BufferPool _pool;

        public double this[int index] => _queue[index];
        public int Start => 0;
        public int End => _queue.Count;

        public int Capacity
        {
            get => _queue.Span.Length;
            set
            {
                if (value <= 0) Console.WriteLine("Capacity must be positive");
                if (Capacity != value)
                    _queue.Resize(value, _pool);
            }
        }

        public TimingsRingBuffer(int maximumCapacity, BufferPool pool)
        {
            if (maximumCapacity <= 0) return;
            _pool = pool;
            _queue = new QuickQueue<double>(maximumCapacity, pool);
        }

        public void Add(double time)
        {
            if (_queue.Count == Capacity) _queue.Dequeue();
            _queue.EnqueueUnsafely(time);
        }

        public void Dispose()
        {
            _queue.Dispose(_pool);
        }
    }
}
