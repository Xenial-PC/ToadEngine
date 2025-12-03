using Assimp;

namespace ToadEngine.Classes.Textures.Base;

public class BaseTextures
{
    public static Texture Black => Texture.FromEmbed("black.png", TextureType.Diffuse)!;
    public static Texture White => Texture.FromEmbed("white.jpg", TextureType.Diffuse)!;
    public static Texture Gray => Texture.FromEmbed("gray.jpg", TextureType.Diffuse)!;
    public static Texture Blue => Texture.FromEmbed("blue.jpg", TextureType.Diffuse)!;
    public static Texture Red => Texture.FromEmbed("red.png", TextureType.Diffuse)!;
    public static Texture Yellow => Texture.FromEmbed("yellow.jpg", TextureType.Diffuse)!;
    public static Texture Green => Texture.FromEmbed("green.jpg", TextureType.Diffuse)!;
}
