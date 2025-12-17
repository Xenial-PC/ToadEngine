using Assimp;
using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Base.Scripting.Renderer;
using ToadEngine.Classes.Textures;
using Material = ToadEngine.Classes.Base.Assets.Material;

namespace SimplePlatformer.Classes.GameObjects.Models;

public class Meteor : MonoBehavior
{
    public MeshRenderer MeshRenderer = null!;

    public void Awake()
    {
        MeshRenderer = AddComponent<MeshRenderer>();
        MeshRenderer.Model =
            AssetManager.LoadModel($"{Directory.GetCurrentDirectory()}/Resources/Models/Meteor/source/", "Meteor.obj",
                materials: [AssetManager.GetMaterial("MeteorMat")]);
    }
}
