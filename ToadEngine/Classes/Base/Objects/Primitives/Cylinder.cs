using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Objects.Primitives;

public class Cylinder : GameObject
{
    private Model _cylinderModel;

    public override void Setup()
    {
        _cylinderModel = new Model("Cylinder.obj");
    }

    public override void Draw()
    {
        CoreShader.Use();
        UpdateModelMatrix();

        var camera = Service.MainCamera;

        CoreShader.SetMatrix4("model", Model);
        CoreShader.SetMatrix4("view", camera.GetViewMatrix());
        CoreShader.SetMatrix4("projection", camera.GetProjectionMatrix());
        CoreShader.SetVector3("viewPos", camera.Transform.LocalPosition);

        _cylinderModel.Draw(CoreShader);
    }
}
