using Assimp;
using Prowl.Vector;
using StbImageSharp;
using TextureWrapMode = OpenTK.Graphics.OpenGL4.TextureWrapMode;

namespace ToadEngine.Classes.Textures;

public class Texture
{
    public int Handle;
    public TextureType Type;
    public string Path;

    private static readonly Dictionary<string, int> LoadedTextures = new();
    private static readonly List<Texture> LoadedTexturesList = new();

    public uint Width;
    public uint Height;

    public static Texture FromPath(string imagePath, TextureType type)
    {
        if (LoadedTextures.TryGetValue(imagePath, out var existingHandle))
        {
            var tex = LoadedTexturesList.Find(tx => tx.Handle == existingHandle);
            return tex ?? null!;
        }

        if (!File.Exists(imagePath))
        {
            Console.WriteLine($"Error: File '{imagePath}' does not exist!");
            return new Texture();
        }

        var texture = new Texture
        {
            Handle = GL.GenTexture(),
            Type = type,
            Path = imagePath
        };
        texture.Use(TextureUnit.Texture0);

        StbImage.stbi_set_flip_vertically_on_load(1);

        using (var stream = File.OpenRead(imagePath))
        {
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, type is TextureType.Specular or TextureType.Normals ? PixelInternalFormat.Rgba : PixelInternalFormat.SrgbAlpha,
                image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        }

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        LoadedTextures[imagePath] = texture.Handle;
        LoadedTexturesList.Add(texture);

        return texture;
    }

    public static Texture? FromEmbed(string imagePath, TextureType type)
    {
        if (LoadedTextures.TryGetValue(imagePath, out var existingHandle))
        {
            var tex = LoadedTexturesList.Find(tx => tx.Handle == existingHandle);
            return tex ?? null!;
        }

        var texture = new Texture
        {
            Handle = GL.GenTexture(),
            Type = type,
            Path = imagePath
        };
        texture.Use(TextureUnit.Texture0);

        StbImage.stbi_set_flip_vertically_on_load(1);

        using (var stream = RReader.ReadAsMemoryStream(imagePath))
        {
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, type is TextureType.Specular or TextureType.Normals ? PixelInternalFormat.Rgba : PixelInternalFormat.SrgbAlpha,
                image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        }

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        LoadedTextures[imagePath] = texture.Handle;
        LoadedTexturesList.Add(texture);

        return texture;
    }

    public static Texture CreateNew(uint width, uint height)
    {
        var handle = GL.GenTexture();
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, handle);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, (int)width, (int)height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        return new Texture { Handle = handle, Width = width, Height = height };
    }

    public void SetData(IntRect bounds, byte[] data)
    {
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
        GL.TexSubImage2D(TextureTarget.Texture2D, 0, bounds.Min.X, bounds.Min.Y, bounds.Size.X, bounds.Size.Y, PixelFormat.Rgba, PixelType.UnsignedByte, data);
    }


    public void Use(TextureUnit unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public static void ClearTextures()
    {
        LoadedTextures.Clear();
        LoadedTexturesList.Clear();
    }
}