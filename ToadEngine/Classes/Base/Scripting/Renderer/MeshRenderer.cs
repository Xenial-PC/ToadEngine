using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Scripting.Renderer;

public class MeshRenderer : MonoBehavior, IRenderObject
{
    public Model Model { get; set; } = null!;

    public void Draw()
    {
        CoreShader.Use();
        GameObject.UpdateModelMatrix();

        CoreShader.SetMatrix4("model", GameObject.Model);
        Model.Draw(CoreShader);
    }
}
