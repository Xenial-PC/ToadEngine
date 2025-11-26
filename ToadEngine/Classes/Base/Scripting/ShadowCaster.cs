using System.Diagnostics;
using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Shaders;

namespace ToadEngine.Classes.Base.Scripting;

public class ShadowCaster : Behaviour
{
    public int CasterFBO, ShadowMap;
    public int ShadowWidth = 1024, ShadowHeight = 1024;
    public Matrix4 LightSpaceMatrix;
    public bool IsCastingShadows;

    public float Distance = 80f, 
        SceneSize = 50f, 
        NearPlane = 1f, 
        FarPlane = 200f;

    public override void Setup()
    {
        GL.GenFramebuffers(1, out CasterFBO);

        GL.GenTextures(1, out ShadowMap);
        GL.BindTexture(TextureTarget.Texture2D, ShadowMap);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, ShadowWidth, ShadowHeight, 0, PixelFormat.DepthComponent, PixelType.Float, nint.Zero);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

        var borderColor = new[] { 1.0f, 1.0f, 1.0f, 1.0f };
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, CasterFBO);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, ShadowMap, 0);
        GL.DrawBuffer(DrawBufferMode.None);
        GL.ReadBuffer(ReadBufferMode.None);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void ConfigureShaderAndMatrices()
    {
        var lightView = new Matrix4();
        var lightProjection = new Matrix4();

        switch (GameObject)
        {
            case DirectionLight directionLight:
                var lightPos = -Vector3.Normalize(GameObject.Transform.Rotation) * Distance;
                lightProjection = Matrix4.CreateOrthographicOffCenter(
                    -SceneSize, SceneSize,
                    -SceneSize, SceneSize,
                    NearPlane, FarPlane
                );
                lightView = Matrix4.LookAt(lightPos, Vector3.Zero, -Vector3.UnitY);
                break;
            case PointLight pointLight: // TODO: Implement later
                lightPos = -Vector3.Normalize(GameObject.Transform.Rotation) * Distance;
                lightProjection = Matrix4.CreateOrthographicOffCenter(
                    -SceneSize, SceneSize,
                    -SceneSize, SceneSize,
                    NearPlane, FarPlane
                );
                lightView = Matrix4.LookAt(lightPos, Vector3.Zero, -Vector3.UnitY);
                break;
            case SpotLight spotLight:
                lightPos = -Vector3.Normalize(GameObject.Transform.Rotation) * Distance;
                lightProjection = Matrix4.CreateOrthographicOffCenter(
                    -SceneSize, SceneSize,
                    -SceneSize, SceneSize,
                    NearPlane, FarPlane
                );
                lightView = Matrix4.LookAt(lightPos, Vector3.Zero, -Vector3.UnitY);
                break;
        }
        LightSpaceMatrix = lightView * lightProjection;
    }
}
