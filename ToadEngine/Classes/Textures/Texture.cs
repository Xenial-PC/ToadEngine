using Assimp;
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