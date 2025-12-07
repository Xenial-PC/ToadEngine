using ToadEngine.Classes.Base.Rendering.Object;

namespace ToadEngine.Classes.Base.Physics;

/// <summary>
/// Holds detection layers for physics objects
/// </summary>
public class PhysicsLayer
{
    public int Layer { get; set; } = (int)PhysicsLayers.None;
    public List<int> IgnoredObjects { get; } = new();

    private static readonly Dictionary<int, List<int>> LayersToIgnore = new();
    private static readonly Dictionary<GameObject, List<GameObject>> ObjectsToIgnore = new();

    public static bool ShouldCollideLayer(int layerA, int layerB) => !(LayersToIgnore.TryGetValue(layerA, out var layer) && layer.Contains(layerB));

    public static bool ShouldCollideObject(GameObject gameObject, GameObject gameObjectToIgnore)
    {
        if (!ObjectsToIgnore.TryGetValue(gameObject, out var goList)) return true;
        return !goList.Contains(gameObjectToIgnore);
    }

    public void AddIgnoredLayer(int layer, int layerToIgnore)
    {
        if (LayersToIgnore.TryGetValue(layer, out var value))
        {
            value.Add(layerToIgnore);
            return;
        }

        LayersToIgnore.Add(layer, [layerToIgnore]);
    }

    public void RemoveLayer(int layer)
    {
        if (!LayersToIgnore.ContainsKey(layer)) return;
        LayersToIgnore.Remove(layer);
    }

    public void RemoveIgnoredLayer(int layer, int layerToRemove)
    {
        if (!LayersToIgnore.TryGetValue(layer, out var layerList)) return;
        layerList.Remove(layerToRemove);
    }

    public void AddIgnoredObject(GameObject go, GameObject gameObjectToIgnore)
    {
        if (ObjectsToIgnore.TryGetValue(go, out var goList))
        {
            if (!goList.Contains(gameObjectToIgnore))
                goList.Add(gameObjectToIgnore);
            return;
        }

        ObjectsToIgnore.Add(go, [gameObjectToIgnore]);
    }

    public void RemoveObject(GameObject go)
    {
        if (!ObjectsToIgnore.ContainsKey(go)) return;
        ObjectsToIgnore.Remove(go);
    }

    public void RemoveIgnoredObject(GameObject go, GameObject gameObjectToRemove)
    {
        if (!ObjectsToIgnore.TryGetValue(go, out var objList)) return;
        objList.Remove(gameObjectToRemove);
    }

    public static void Reset()
    {
        LayersToIgnore.Clear();
        ObjectsToIgnore.Clear();
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
