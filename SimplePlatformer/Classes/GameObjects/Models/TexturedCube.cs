using Assimp;
using ToadEngine.Classes.Base.Objects.Primitives;
using ToadEngine.Classes.Textures;
using Material = ToadEngine.Classes.Base.Assets.Material;

namespace SimplePlatformer.Classes.GameObjects.Models;

public class TexturedCube(string diffuse = "", string specular = "", string normal = "") : Cube
{
    public override void Setup()
    {
        base.Setup();
        Mesh.Model.SetMaterials([
            new Material()
            {
                Diffuse = Texture.FromPath(diffuse, TextureType.Diffuse),
                Specular = Texture.FromPath(specular, TextureType.Specular),
                Normal = Texture.FromPath(normal, TextureType.Normals)
            }
        ]);
    }
}
