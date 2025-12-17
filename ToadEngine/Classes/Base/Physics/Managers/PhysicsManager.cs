namespace ToadEngine.Classes.Base.Physics.Managers;

public class PhysicsManager
{
    private static readonly Dictionary<string, PhysicsSimulation> Physics = new();

    public static PhysicsSimulation Register(string name)
    {
        if (Physics.TryGetValue(name, out var simulation))
            return simulation;

        simulation = new PhysicsSimulation();
        Physics.TryAdd(name, simulation);

        return simulation;
    }

    public static PhysicsSimulation Get(string name) => Physics.TryGetValue(name, out var simulation) ? simulation : Register(name);
    public static List<PhysicsSimulation> GetSimulations => Physics.Select(s => s.Value).ToList();

    public static void Reset()
    {
        foreach (var simulation in GetSimulations) simulation.Dispose();
        Physics.Clear();
    }
}