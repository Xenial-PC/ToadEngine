using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Shaders;
using ToadEngine.Classes.Textures;

namespace ToadEngine.Classes.Base.Assets;

public class Material
{
    public Shader Shader = Service.CoreShader;
    public Texture? Diffuse;
    public Texture? Specular;
    public Texture? Normal;
    public Texture? Height;
}
