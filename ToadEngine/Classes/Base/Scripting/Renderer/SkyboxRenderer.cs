using Prowl.Echo;
using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Shaders;

namespace ToadEngine.Classes.Base.Scripting.Renderer;

public class SkyboxRenderer : RenderObject
{
    [SerializeField] public Shader SkyboxShader = null!;
    [SerializeField] public int SkyboxVAO;

    public override void Draw()
    {
        var camera = Camera.MainCamera;

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

    public void Dispose()
    {
        SkyboxShader?.Dispose();
    }
}
