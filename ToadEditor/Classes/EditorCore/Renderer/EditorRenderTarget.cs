using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Textures;

namespace ToadEditor.Classes.EditorCore.Renderer;

public class EditorRenderTarget : IRenderTarget
{
    public int FBO;
    public Texture Texture;
    public int DepthRBO;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public void Bind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
        GL.Viewport(0, 0, Width, Height);
    }

    public void Unbind()
    {
        GL.Viewport(0, 0, Service.Window.Width, Service.Window.Height);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public Texture Resize(int width, int height)
    {
        var tex = new Texture();

        Width = width;
        Height = height;

        if (FBO != 0) GL.DeleteFramebuffer(FBO);
        if (DepthRBO != 0) GL.DeleteRenderbuffer(DepthRBO);

        FBO = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);

        tex.Handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, tex.Handle);

        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            Width,
            Height,
            0,
            PixelFormat.Rgba,
            PixelType.UnsignedByte,
            IntPtr.Zero
        );

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        GL.FramebufferTexture2D(
            FramebufferTarget.Framebuffer,
            FramebufferAttachment.ColorAttachment0,
            TextureTarget.Texture2D,
            tex.Handle,
            0
        );

        GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
        GL.ReadBuffer(ReadBufferMode.ColorAttachment0);

        DepthRBO = GL.GenRenderbuffer();
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthRBO);
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, Width, Height);

        GL.FramebufferRenderbuffer(
            FramebufferTarget.Framebuffer,
            FramebufferAttachment.DepthAttachment,
            RenderbufferTarget.Renderbuffer,
            DepthRBO
        );

        var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        if (status != FramebufferErrorCode.FramebufferComplete)
            Console.WriteLine($"[EditorRenderTarget] Framebuffer incomplete: {status}");

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        return tex;
    }
}
