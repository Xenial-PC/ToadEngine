using StbImageSharp;

namespace ToadEngine.Classes.Textures;

public class CubeMap
{
    public int Handle;

    public CubeMap(List<string> textures)
    {
        LoadCubeMap(textures);
    }

    public void LoadCubeMap(List<string> faces)
    {
        StbImage.stbi_set_flip_vertically_on_load(0);

        GL.GenTextures(1, out int texId);
        GL.BindTexture(TextureTarget.TextureCubeMap, texId);
        
        for (var i = 0; i < faces.Count; i++)
        {
            using var stream = File.OpenRead(faces[i]);
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlue);

            if (image.Data.Length > 0)
            {
                GL.TexImage2D((TextureTarget)((int)TextureTarget.TextureCubeMapPositiveX + i), 0, PixelInternalFormat.SrgbAlpha,
                    image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, image.Data);
                continue;
            }

            Console.WriteLine($"Failed to load CubeMap texture at path: {faces[i]}");
        }

        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)All.Linear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)All.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)All.ClampToEdge);

        GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);

        Handle = texId;
    }
}
