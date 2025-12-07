namespace ToadEngine.Classes.Base.Physics;

public class TriggerRegistry
{
    public static readonly HashSet<int> Triggers = new();
    public static bool IsTrigger(int handle) => Triggers.Contains(handle);
}

public static class TriggerManager
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