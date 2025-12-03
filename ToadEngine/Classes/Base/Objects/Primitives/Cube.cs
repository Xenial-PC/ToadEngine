using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Objects.Primitives;

public class Cube : GameObject
{
    public Model CubeModel;

    public override void Setup()
    {
        CubeModel = new Model("Cube.obj");
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

        CubeModel.Draw(CoreShader);
    }
}
