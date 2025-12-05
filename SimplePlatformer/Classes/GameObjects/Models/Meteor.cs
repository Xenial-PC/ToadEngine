using Assimp;
using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Renderer;
using ToadEngine.Classes.Textures;
using Material = ToadEngine.Classes.Base.Assets.Material;

namespace SimplePlatformer.Classes.GameObjects.Models;

public class Meteor : GameObject
{
    public MeshRenderer MeshRenderer = null!;

    public override void Setup()
    {
        MeshRenderer = AddComponent<MeshRenderer>();
        MeshRenderer.Model =
            AssetManager.LoadModel($"{Directory.GetCurrentDirectory()}/Resources/Models/Meteor/source/", "Meteor.obj", [
                AssetManager.GetMaterial("MeteorMat")
            ]);
    }
}
