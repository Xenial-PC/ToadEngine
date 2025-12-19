using Prowl.Echo;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Textures.Base;

namespace ToadEngine.Classes.Base.Assets;

public class AssetManager
{
    [SerializeField] public readonly Dictionary<ModelKey, Model> Models = new();
    [SerializeField] public readonly Dictionary<string, Material> Materials = new();

    private static AssetManager Assets => Service.AssetManager;

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
        if (Assets.Models.TryGetValue(key, out var m) && key.MaterialHash != -1) return m;

        m = Model.Load(path, model);

        if (materials != null) m.SetMaterials(materials);
        if (key.MaterialHash != -1) Assets.Models[key] = m;
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
        if (Assets.Models.TryGetValue(key, out var m) && key.MaterialHash != -1) return m;

        m = Model.Load(model);

        if (materials != null) m.SetMaterials(materials);
        if (key.MaterialHash != -1) Assets.Models[key] = m;
        return m;
    }

    /// <summary>
    /// Loads a material into memory
    /// </summary>
    /// <param name="matName"></param>
    /// <param name="material"></param>
    /// <returns></returns>
    public static Material CreateMaterial(string matName, Material material)
    {
        if (Assets.Materials.TryGetValue(matName, out var mat))
            return mat;

        Assets.Materials.Add(matName, material);
        return material;
    }

    /// <summary>
    /// Gets an existing material, if it fails it returns default White material
    /// </summary>
    /// <param name="matName"></param>
    /// <returns>Material / Default White Material</returns>
    public static Material GetMaterial(string matName)
    {
        if (!Assets.Materials.TryGetValue(matName, out var material))
            return new Material()
            {
                Diffuse = BaseTextures.White
            };

        return material;
    }

    public void Reset()
    {
        Models.Clear();
        Materials.Clear();
    }
}