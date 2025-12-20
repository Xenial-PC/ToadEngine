using Prowl.Echo;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Shaders;

public class ShaderManager : ISerializable
{
    public static Dictionary<string, Shader> ShaderCache = new();

    public static Shader Add(string name, string vertex, string fragment)
    {
        if (ShaderCache.TryGetValue(name, out var shader))
            return shader;

        shader = Shader.LoadShader(name, vertex, fragment);
        ShaderCache.Add(name, shader);
        return shader;
    }

    public static Shader? Get(string name) => ShaderCache.GetValueOrDefault(name);
    
    public static void Remove(string name) => ShaderCache.Remove(name);

    public void Reset()
    {
        foreach (var shaderCacheValue in ShaderCache)
            shaderCacheValue.Value.Dispose();

        ShaderCache.Clear();
    }

    public void Serialize(ref EchoObject compound, SerializationContext ctx)
    {
        var shaderCache = Serializer.Serialize(ShaderCache, ctx);
        compound.Add("ShaderCache", shaderCache);
    }

    public void Deserialize(EchoObject value, SerializationContext ctx)
    {
        var shaderCacheObject = value.Get("ShaderCache");
        var shaderCache = Serializer.Deserialize<Dictionary<string, Shader>>(shaderCacheObject, ctx);

        foreach (var shader in shaderCache)
        {
            if (!ShaderCache.ContainsKey(shader.Key))
                Add(shader.Value.Name, shader.Value.Vert, shader.Value.Frag);

            shader.Value.Dispose();
        }
    }
}
