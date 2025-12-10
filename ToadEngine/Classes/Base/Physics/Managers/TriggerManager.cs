namespace ToadEngine.Classes.Base.Physics.Managers;

public class TriggerRegistry
{
    public static readonly HashSet<int> Triggers = new();
    public static bool IsTrigger(int handle) => Triggers.Contains(handle);
}

public static class TriggerManager
{
    private static readonly HashSet<(int, int)> ActiveOverlaps = new();
    private static readonly HashSet<(int, int)> FrameOverlaps = new();
    private static readonly Dictionary<(int, int), int> MissingFrameCount = new();

    private const int MaxMissedFrames = 5;

    public static Action<int, int>? OnEnter;
    public static Action<int, int>? OnExit;

    public static void RegisterOverlap(int a, int b)
    {
        if (a > b) (a, b) = (b, a);
        FrameOverlaps.Add((a, b));

        if (ActiveOverlaps.Add((a, b)))
            OnEnter?.Invoke(a, b);
    }

    public static void EndFrame()
    {
        foreach (var pair in ActiveOverlaps)
        {
            if (!FrameOverlaps.Contains(pair))
            {
                if (!MissingFrameCount.TryGetValue(pair, out var count))
                {
                    MissingFrameCount[pair] = 1;
                    continue;
                }

                MissingFrameCount[pair] = count + 1;
                if (MissingFrameCount[pair] < MaxMissedFrames) continue;
                
                OnExit?.Invoke(pair.Item1, pair.Item2);
                ActiveOverlaps.Remove(pair);
                continue;
            }

            MissingFrameCount[pair] = 0;
        }

        FrameOverlaps.Clear();
    }

    public static bool IsActive(int a, int b)
        => ActiveOverlaps.Contains((a, b));

    public static void Reset()
    {
        ActiveOverlaps.Clear();
        FrameOverlaps.Clear();
        MissingFrameCount.Clear();
        TriggerRegistry.Triggers.Clear();
    }
}