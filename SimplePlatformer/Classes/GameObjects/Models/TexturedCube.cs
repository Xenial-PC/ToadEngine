using ToadEngine.Classes.Base.Objects.Primitives;
using ToadEngine.Classes.Base.Scripting.Base;
using Material = ToadEngine.Classes.Base.Assets.Material;

namespace SimplePlatformer.Classes.GameObjects.Models;

public class TexturedCube : MonoBehavior
{
    public Material Material = null!;
    public CubeMesh CubeMesh = null!;

    public void Awake()
    {
        CubeMesh = GetComponent<CubeMesh>()!;
        CubeMesh.Mesh.Model.SetMaterials([Material]);
    }
}
