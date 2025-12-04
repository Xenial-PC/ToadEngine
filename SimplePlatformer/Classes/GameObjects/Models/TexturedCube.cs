using Assimp;
using ToadEngine.Classes.Base.Objects.Primitives;
using ToadEngine.Classes.Textures;

namespace SimplePlatformer.Classes.GameObjects.Models;

public class TexturedCube(string diffuse = "", string specular = "", string normal = "") : Cube
{
    public override void Setup()
    {
        base.Setup();
        if (diffuse != string.Empty) Mesh.Model.GetTextures[0].Add(Texture.FromPath(diffuse, TextureType.Diffuse));
        if (specular != string.Empty) Mesh.Model.GetTextures[0].Add(Texture.FromPath(specular, TextureType.Specular));
        if (normal != string.Empty) Mesh.Model.GetTextures[0].Add(Texture.FromPath(normal, TextureType.Normals));
    }
}
