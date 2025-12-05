using SimplePlatformer.Classes.GameObjects.Models;

namespace SimplePlatformer.Classes.GameObjects.World;

public class BasePlate
{
    public TexturedCube GameObject { get; private set; }

    public BasePlate(string? name = null, Vector3? size = null)
    {
        Load(name, size);
    }

    private void Load(string? name = null, Vector3? size = null)
    {
        GameObject = new TexturedCube(
            diffuse: $"{Directory.GetCurrentDirectory()}/Resources/Textures/concrete.jpg",
            specular: $"{Directory.GetCurrentDirectory()}/Resources/Textures/concrete_specular.jpg",
            normal: $"{Directory.GetCurrentDirectory()}/Resources/Textures/concrete_normal.png");

        if (size != null) GameObject.Transform.LocalScale = (Vector3)size;

        var collider = GameObject.AddComponent<BoxCollider>();
        collider.Type = ColliderType.Kinematic;
        collider.Size = size ?? GameObject.Transform.LocalScale;
    }
}
