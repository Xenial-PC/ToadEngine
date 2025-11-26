using System.Drawing;
using Vector4 = System.Numerics.Vector4;

#pragma warning disable CS1591

namespace ToadEngine.Classes.DearImGui.OpenTK.Extensions;

public static class ColorExtensions
{
    public static Vector4 ToVector4(this Color color)
    {
        const float f = 1.0f / 255.0f;

        var vector4 = new Vector4(color.R * f, color.G * f, color.B * f, color.A * f);

        return vector4;
    }
}