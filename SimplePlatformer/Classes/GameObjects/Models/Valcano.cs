using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Base.Scripting.Renderer;

namespace SimplePlatformer.Classes.GameObjects.Models;

public class Volcano : MonoBehavior
{
    public MeshRenderer MeshRenderer;

    public void Awake()
    {
        MeshRenderer = AddComponent<MeshRenderer>();
        MeshRenderer.Model = AssetManager.LoadModel($"{Directory.GetCurrentDirectory()}/Resources/Models/Volcano/source/", "Volcano.fbx",
            materials: [AssetManager.GetMaterial("VolcanoMat")]);

        Transform.Rotation = new Vector3(-90, 0, 0);
    }
}
