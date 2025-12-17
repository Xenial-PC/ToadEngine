using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Base.Scripting.Renderer;

namespace ToadEngine.Classes.Base.Objects.Primitives;

public class SphereMesh : MonoBehavior
{
    public MeshRenderer Mesh = null!;

    public void Awake()
    {
        Mesh = AddComponent<MeshRenderer>();
        Mesh.Model = AssetManager.LoadModel("Sphere.obj");
    }
}
