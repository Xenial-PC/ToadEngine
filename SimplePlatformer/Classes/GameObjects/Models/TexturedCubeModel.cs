using Assimp;
using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Objects.Primitives;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Textures;
using Camera = ToadEngine.Classes.Base.Objects.View.Camera;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace SimplePlatformer.Classes.GameObjects.Models;

public class TexturedCubeModel(string diffuse = "", string specular = "", string normal = "") : Cube
{
    public BaseLight.Material Material = new()
    {
        Diffuse = 0,
        Specular = 1,
        Normal = 2,
        Shininess = 32.0f
    };

    public override void Setup()
    {
        base.Setup();
        if (diffuse != string.Empty) CubeModel.GetTextures[0].Add(Texture.FromPath(diffuse, TextureType.Diffuse));
        if (specular != string.Empty) CubeModel.GetTextures[0].Add(Texture.FromPath(specular, TextureType.Specular));
        if (normal != string.Empty) CubeModel.GetTextures[0].Add(Texture.FromPath(normal, TextureType.Normals));
    }
}
