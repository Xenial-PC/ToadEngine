using System.Reflection.PortableExecutable;
using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Objects.Skybox;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Shaders;

namespace ToadEngine.Classes.Base.Scripting
{
    public class SkyboxRenderer : Behavior, IRenderObject
    {
        public Shader SkyboxShader = null!;
        public int SkyboxVAO;

        public void Draw()
        {
            var camera = Service.MainCamera;

            GL.DepthFunc(DepthFunction.Lequal);
            GL.DepthMask(false);
            SkyboxShader!.Use();

            SkyboxShader.SetMatrix4("view", new Matrix4(new Matrix3(camera.GetViewMatrix())));
            SkyboxShader.SetMatrix4("projection", camera.GetProjectionMatrix());

            GL.BindVertexArray(SkyboxVAO);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, SkyboxShader!.Handle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);

            GL.DepthMask(true);
            GL.DepthFunc(DepthFunction.Less);
        }

        public override void OnDispose()
        {
            SkyboxShader?.Dispose();
        }
    }
}
