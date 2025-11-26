using System.Diagnostics;
using System.Reflection;

namespace ToadEngine.Classes.Extensions;

public static class RReader
{
    public static string ReadText(string resourceName)
    {
        var resPath = GetResourcePathByName(resourceName);
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(resPath);
        if (stream == null) return string.Empty;

        using var sReader = new StreamReader(stream);
        var fileContents = sReader.ReadToEnd();

        return fileContents;
    }

    public static byte[]? ReadBytes(string resourceName)
    {
        var resPath = GetResourcePathByName(resourceName);
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(resPath);
        if (stream == null) return null;

        using var mReader = new MemoryStream();
        stream.CopyTo(mReader);
        
        var fileContents = mReader.ToArray();
        return fileContents;
    }

    public static Stream GetStream(string resourceName)
    {
        var resPath = GetResourcePathByName(resourceName);
        var assembly = Assembly.GetExecutingAssembly();

        var stream = assembly.GetManifestResourceStream(resPath);
        return stream ?? null!;
    }

    private static string GetResourcePathByName(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(res => res.Contains(name));
        return string.IsNullOrEmpty(resourceName) ? string.Empty : resourceName;
    }
}
