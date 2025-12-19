using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Shaders;

public class ShaderManager
{
    public Dictionary<string, Shader> ShaderCache = new();

    public static Shader Add(string name, string vertex, string fragment)
    {
        if (Service.ShaderManager.ShaderCache.TryGetValue(name, out var shader)) return shader;
        shader = Shader.LoadShader(name, vertex, fragment);
        Service.ShaderManager.ShaderCache.Add(name, shader);
        return shader;
    }

    public static void Remove(string name) => Service.ShaderManager.ShaderCache.Remove(name);
    
    public static void Reset() => Service.ShaderManager.ShaderCache.Clear();
}
