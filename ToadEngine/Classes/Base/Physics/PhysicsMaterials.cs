using BepuPhysics.Constraints;

namespace ToadEngine.Classes.Base.Physics;

public class PhysicsMaterials
{
    public static PhysicsMaterial Default { get; } = PhysicsMaterialRegistry.Register(new PhysicsMaterial
    {
        Name = "Default",
        Friction = 1f,
        Restitution = 8f,
        SpringSettings = new SpringSettings(30, 1)
    });
}
