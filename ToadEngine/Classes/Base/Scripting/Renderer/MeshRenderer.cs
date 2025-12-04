using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Scripting.Renderer
{
    public class MeshRenderer : Behavior, IRenderObject
    {
        public Model Model { get; set; } = null!;
        

        public void Draw()
        {
            if (GameObject == null) return;

            CoreShader.Use();
            GameObject.UpdateModelMatrix();

            var camera = Service.MainCamera;

            CoreShader.SetMatrix4("model", GameObject.Model);
            CoreShader.SetMatrix4("view", camera.GetViewMatrix());
            CoreShader.SetMatrix4("projection", camera.GetProjectionMatrix());
            CoreShader.SetVector3("viewPos", camera.Transform.LocalPosition);

            Model.Draw(CoreShader);
        }
    }
}
