using StbImageSharp;

namespace ToadEngine.Classes.Textures;

public class Texture
{
    public int Handle;
    public string Type;
    public string Path;

    private static readonly Dictionary<string, int> LoadedTextures = new();

    public Texture(string imagePath, bool isSpecularOrNormalMap)
    {
        Path = imagePath;
        if (LoadedTextures.TryGetValue(imagePath, out var existingHandle))
        {
            Handle = existingHandle;
            return;
        }

        if (!File.Exists(imagePath))
        {
            Console.WriteLine($"Error: File '{imagePath}' does not exist!");
            return;
        }

        Handle = GL.GenTexture();
        Use(TextureUnit.Texture0);

        StbImage.stbi_set_flip_vertically_on_load(1);

        using (var stream = File.OpenRead(imagePath))
        {
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, isSpecularOrNormalMap ? PixelInternalFormat.Rgba : PixelInternalFormat.SrgbAlpha,
                image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        }

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        LoadedTextures[imagePath] = Handle;
    }

    public void Use(TextureUnit unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }
}