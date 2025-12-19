using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Rendering.Object;

namespace ToadEngine.Classes.Base.Scripting.Renderer;

public class MeshRenderer : RenderObject
{
    public Model Model = null!;

    public override void Draw()
    {
        CoreShader.Use();
        GameObject.UpdateModelMatrix();

        CoreShader.SetMatrix4("model", GameObject.Model);
        Model.Draw(CoreShader);
    }
}
