using BepuPhysics.Constraints;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Physics;

/// <summary>
/// Holds Objects Physics Data: Name, Friction, etc. For custom behavior for objects
/// </summary>
public class PhysicsMaterial
{
    private float _gravity = Service.Physics.Settings.Gravity.Y * 1f;

    public string Name { get; set; } = null!;
    public float Friction { get; set; } = 1f;
    public float Restitution { get; set; } = 8f;

    /// <summary>
    /// The scalar for the overall gravity
    /// </summary>
    public float Gravity
    {
        get => _gravity;
        set => _gravity = Service.Physics.Settings.Gravity.Y * (value);
    }

    public SpringSettings SpringSettings { get; set; } = new(30, 1);
    public PhysicsLayer PhysicsLayer { get; set; } = new() { Layer = (int)PhysicsLayers.None };
}

/// <summary>
/// Registry for physics materials, and connected handles
/// </summary>
public class PhysicsMaterialRegistry
{
    private static readonly Dictionary<string, PhysicsMaterial> Materials = new();
    private static readonly Dictionary<int, PhysicsMaterial> RegisteredMaterials = new();

    /// <summary>
    /// Registers base physics material so it can be used globally
    /// </summary>
    /// <param name="material"></param>
    /// <returns></returns>
    public static PhysicsMaterial Register(PhysicsMaterial material)
    {
        if (Materials.TryGetValue(material.Name, out var physicsMaterial))
            return physicsMaterial;

        Materials.TryAdd(material.Name, material);
        return material;
    }

    /// <summary>
    /// Registers object handle for the bound physics material
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="material"></param>
    /// <returns></returns>
    public static PhysicsMaterial Register(int handle, PhysicsMaterial material)
    {
        if (RegisteredMaterials.TryGetValue(handle, out var physicsMaterial))
            return physicsMaterial;

        RegisteredMaterials.TryAdd(handle, Register(material));
        return material;
    }

    /// <summary>
    /// Gets base material from the material registry
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static PhysicsMaterial? Get(string name) => Materials.GetValueOrDefault(name);

    /// <summary>
    /// Gets material registered to the object
    /// </summary>
    /// <param name="handle"></param>
    /// <returns></returns>
    public static PhysicsMaterial? Get(int handle) => RegisteredMaterials.GetValueOrDefault(handle);

    /// <summary>
    /// Sets bound material of a physics object to another material
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="mat"></param>
    public static void Set(int handle, PhysicsMaterial mat)
    {
        var material = Get(handle);
        if (material == null) return;
        RegisteredMaterials[handle] = mat;
    }
}
