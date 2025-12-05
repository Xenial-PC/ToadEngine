using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Textures;
using Assimp;

namespace SimplePlatformer.Classes.GameObjects.Materials;

public class SceneMaterials
{
    public static void LoadMaterials()
    {
        AssetManager.CreateMaterial("LavaMat", new ToadEngine.Classes.Base.Assets.Material()
        {
            Diffuse = Texture.FromPath($"{Directory.GetCurrentDirectory()}/Resources/Textures/lava/lava.png", TextureType.Diffuse),
            Specular = Texture.FromPath($"{Directory.GetCurrentDirectory()}/Resources/Textures/lava/lava_roughness.png", TextureType.Specular),
            Normal = Texture.FromPath($"{Directory.GetCurrentDirectory()}/Resources/Textures/lava/lava_normal.png", TextureType.Normals)
        });

        AssetManager.CreateMaterial("MeteorMat", new ToadEngine.Classes.Base.Assets.Material()
        {
            Diffuse = Texture.FromPath(
                        $"{Directory.GetCurrentDirectory()}/Resources/Models/Meteor/textures/meteor.png",
                        TextureType.Diffuse),

            Specular = Texture.FromPath(
                        $"{Directory.GetCurrentDirectory()}/Resources/Models/Meteor/textures/meteor_specular.png",
                        TextureType.Specular),

            Normal = Texture.FromPath(
                        $"{Directory.GetCurrentDirectory()}/Resources/Models/Meteor/textures/meteor_normal.png",
                        TextureType.Normals),
        });

        AssetManager.CreateMaterial("VolcanoMat", new ToadEngine.Classes.Base.Assets.Material()
        {
            Diffuse = Texture.FromPath(
                    $"{Directory.GetCurrentDirectory()}/Resources/Models/Volcano/textures/volcano_diffuse.png",
                    TextureType.Diffuse),

            Normal = Texture.FromPath(
                    $"{Directory.GetCurrentDirectory()}/Resources/Models/Volcano/textures/volcano_normal.png",
                    TextureType.Normals)
        });

        AssetManager.CreateMaterial("ConcreteMat", new ToadEngine.Classes.Base.Assets.Material()
        {
            Diffuse = Texture.FromPath($"{Directory.GetCurrentDirectory()}/Resources/Textures/concrete.jpg", TextureType.Diffuse),
            Specular = Texture.FromPath($"{Directory.GetCurrentDirectory()}/Resources/Textures/concrete_specular.jpg", TextureType.Specular),
            Normal = Texture.FromPath($"{Directory.GetCurrentDirectory()}/Resources/Textures/concrete_normal.png", TextureType.Normals)
        });

        AssetManager.CreateMaterial("GraniteMat", new ToadEngine.Classes.Base.Assets.Material()
        {
            Diffuse = Texture.FromPath($"{Directory.GetCurrentDirectory()}/Resources/Textures/granite.jpg", TextureType.Diffuse),
            Specular = Texture.FromPath($"{Directory.GetCurrentDirectory()}/Resources/Textures/granite_specular.jpg", TextureType.Specular),
            Normal = Texture.FromPath($"{Directory.GetCurrentDirectory()}/Resources/Textures/granite_normal.png", TextureType.Normals)
        });
    }
}