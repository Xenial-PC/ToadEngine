using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Shaders;
using ToadEngine.Classes.Textures;

namespace ToadEngine.Classes.Base.Assets;

public class Material
{
    public Shader Shader { get; set; } = Service.CoreShader;
    public Texture? Diffuse { get; set; }
    public Texture? Specular { get; set; }
    public Texture? Normal { get; set; }
    public Texture? Height { get; set; }
}
