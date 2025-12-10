using BepuUtilities.Collections;
using BepuUtilities.Memory;
using BepuUtilities;
using System.Diagnostics;
using static ToadEngine.Classes.Base.Raycasting.RaycastManager;

namespace ToadEngine.Classes.Base.Raycasting;

public class IntersectionAlgorithm
{
    public string Name = null!;
    public int IntersectionCount;
    public Buffer<RayHit> Results;
    public TimingsRingBuffer Timings = null!;

    public Func<int, IntersectionAlgorithm, int> Worker = null!;
    public Action<int> InternalWorker = null!;
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

    public unsafe void Execute(ref QuickList<Ray> rays, IThreadDispatcher? dispatcher)
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