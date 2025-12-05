using System.IO;
using ToadEngine.Classes.Textures;
using ToadEngine.Classes.Textures.Base;

namespace ToadEngine.Classes.Base.Assets;

public class AssetManager
{
    private static readonly Dictionary<ModelKey, Model> Models = new();
    private static readonly Dictionary<string, Material> Materials = new();

    /// <summary>
    /// Loads model from file
    /// </summary>
    /// <param name="path"></param>
    /// <param name="model"></param>
    /// <param name="materials"></param>
    /// <returns></returns>
    public static Model LoadModel(string path, string model, List<Material>? materials = null)
    {
        var key = new ModelKey(model, materials);
        if (Models.TryGetValue(key, out var m) && key.MaterialHash != -1) return m;

        m = Model.Load(path, model);

        if (materials != null) m.SetMaterials(materials);
        if (key.MaterialHash != -1) Models[key] = m;
        return m;
    }

    /// <summary>
    /// Loads model from embedded file
    /// </summary>
    /// <param name="model"></param>
    /// <param name="materials"></param>
    /// <returns></returns>
    public static Model LoadModel(string model, List<Material>? materials = null)
    {
        var key = new ModelKey(model, materials);
        if (Models.TryGetValue(key, out var m) && key.MaterialHash != -1) return m;

        m = Model.Load(model);

        if (materials != null) m.SetMaterials(materials);
        if (key.MaterialHash != -1) Models[key] = m;
        return m;
    }

    /// <summary>
    /// Loads a material into memory
    /// </summary>
    /// <param name="matName"></param>
    /// <param name="material"></param>
    /// <returns></returns>
    public static Material LoadMaterial(string matName, Material material)
    {
        if (Materials.TryGetValue(matName, out var mat))
            return mat;

        Materials.Add(matName, material);
        return material;
    }

    /// <summary>
    /// Gets an existing material, if it fails it returns default White material
    /// </summary>
    /// <param name="matName"></param>
    /// <returns>Material / Default White Material</returns>
    public static Material GetMaterial(string matName)
    {
        if (!Materials.TryGetValue(matName, out var material))
            return new Material()
            {
                Diffuse = BaseTextures.White
            };

        return material;
    }

    public static void Reset()
    {
        Models.Clear();
        Materials.Clear();
    }
}