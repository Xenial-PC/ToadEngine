namespace ToadEngine.Classes.Base.Physics;

/// <summary>
/// Holds detection layers for physics objects
/// </summary>
public class PhysicsLayer
{
    public int Layer { get; set; } = (int)PhysicsLayers.None;

    private static readonly Dictionary<int, int> LayersToIgnore = new();

    public static void RegisterIgnoredLayer(int layer, int layerToIgnore)
    {
        if (LayersToIgnore.TryGetValue(layer, out var value))
            return;

        LayersToIgnore.Add(layer, layerToIgnore);
    }

    public static void RemoveIgnoredLayer(int layer)
    {
        if (!LayersToIgnore.ContainsKey(layer)) return;
        LayersToIgnore.Remove(layer);
    }

    public static bool ShouldCollide(int layerA, int layerB)
    {
        if (LayersToIgnore.TryGetValue(layerA, out var layer))
            return layerB != layer;

        return false;
    }

    public static void Reset()
    {
        LayersToIgnore.Clear();
    }
}

/// <summary>
/// Enum holding base physic layers
/// </summary>
public enum PhysicsLayers
{
    None = 3510,
    Player = 3511,
    Ground = 3512,
    Wall = 3513,
    Enemy = 3514,
}
