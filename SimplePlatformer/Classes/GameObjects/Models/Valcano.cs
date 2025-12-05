using Assimp;
using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Renderer;
using ToadEngine.Classes.Textures;
using Material = ToadEngine.Classes.Base.Assets.Material;

namespace SimplePlatformer.Classes.GameObjects.Models;

public class Volcano : GameObject
{
    public MeshRenderer MeshRenderer;

    public override void Setup()
    {
        MeshRenderer = AddComponent<MeshRenderer>();
        MeshRenderer.Model = AssetManager.LoadModel($"{Directory.GetCurrentDirectory()}/Resources/Models/Volcano/source/", "Volcano.fbx", [
            AssetManager.GetMaterial("VolcanoMat")
        ]);

        Transform.Rotation = new Vector3(-90, 0, 0);
    }
}
