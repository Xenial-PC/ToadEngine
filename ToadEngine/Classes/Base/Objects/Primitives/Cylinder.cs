using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Base.Scripting.Renderer;

namespace ToadEngine.Classes.Base.Objects.Primitives;

public class Cylinder : GameObject
{
    public MeshRenderer Mesh = null!;

    public override void Setup()
    {
        Mesh = AddComponent<MeshRenderer>();
        Mesh.Model = new Model("Cylinder.obj");
    }
}
