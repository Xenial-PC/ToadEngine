using SimplePlatformer.Classes.GameObjects.Models;

namespace SimplePlatformer.Classes.GameObjects.World;

public class BoxContainer
{
    public TexturedCube GameObject { get; private set; }

    public BoxContainer(string? name = null, Vector3? size = null)
    {
        Load(name, size);
    }

    public void Load(string? name = null, Vector3? size = null)
    {
        GameObject = new TexturedCube(
            diffuse: $"{Directory.GetCurrentDirectory()}/Resources/Textures/container.png",
            specular: $"{Directory.GetCurrentDirectory()}/Resources/Textures/container_specular.png");

        var collider = GameObject.AddComponent<BoxCollider>();
        collider.Type = ColliderType.Kinematic;
        collider.Size = size ?? GameObject.Transform.LocalScale;
    }
}
