using ToadEngine.Classes.Base.Objects.Primitives;
using Material = ToadEngine.Classes.Base.Assets.Material;

namespace SimplePlatformer.Classes.GameObjects.Models;

public class TexturedCube(Material mat) : Cube
{
    public override void Setup()
    {
        base.Setup();
        Mesh.Model.SetMaterials([mat]);
    }
}
