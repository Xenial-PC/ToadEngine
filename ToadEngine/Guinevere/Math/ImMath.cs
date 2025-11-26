using System.Runtime.CompilerServices;

namespace Guinevere;

/// <summary>
/// Provides a collection of mathematical utility functions and constants optimized for performance.
/// </summary>
public static class ImMath
{
    /// <summary>
    /// Determines whether two floating-point numbers are approximately equal, accounting for precision limitations.
    /// </summary>
    /// <param name="value1">The first value to compare.</param>
    /// <param name="value2">The second value to compare.</param>
    /// <returns>True if the absolute difference between the two values is less than the smallest positive single precision number; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ApproximatelyEquals(float value1, float value2)
    {
        return Math.Abs(value1 - value2) < float.Epsilon;
    }

    /// <summary>
    /// Determines whether two floating-point vectors are approximately equal, accounting for precision limitations.
    /// </summary>
    /// <param name="a">The first vector to compare.</param>
    /// <param name="b">The second vector to compare.</param>
    /// <returns>True if all corresponding components of the vectors are approximately equal; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ApproximatelyEquals(Vector2 a, Vector2 b)
    {
        return ApproximatelyEquals(a.X, b.X) && ApproximatelyEquals(a.Y, b.Y);
    }

    /// <summary>
    /// Determines whether two floating-point numbers are approximately equal, accounting for precision limitations.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <returns>True if the absolute difference between the two values is less than the smallest positive single precision number; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ApproximatelyEquals(Vector3 a, Vector3 b)
    {
        return ApproximatelyEquals(a.X, b.X) && ApproximatelyEquals(a.Y, b.Y) && ApproximatelyEquals(a.Z, b.Z);
    }

    /// <summary>
    /// Determines whether two floating-point numbers are approximately equal, accounting for precision limitations.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <returns>True if the absolute difference between the two values is less than the smallest positive single precision number; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ApproximatelyEquals(Vector4 a, Vector4 b)
    {
        return ApproximatelyEquals(a.X, b.X) && ApproximatelyEquals(a.Y, b.Y) && ApproximatelyEquals(a.Z, b.Z) &&
               ApproximatelyEquals(a.W, b.W);
    }

    /// <summary>
    /// Linearly interpolates between two values based on a specified interpolation factor.
    /// </summary>
    /// <param name="a">The start value.</param>
    /// <param name="b">The end value.</param>
    /// <param name="duration">The interpolation factor, typically in the range [0, 1]. A value of 0 will return <paramref name="a"/>, and a value of 1 will return <paramref name="b"/>.</param>
    /// <returns>The interpolated value between <paramref name="a"/> and <paramref name="b"/> based on <paramref name="duration"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Lerp(float a, float b, float duration)
    {
        return (1 - duration) * a + duration * b;
    }

    /// <summary>
    /// A constant factor used to convert an angle measurement from degrees to radians.
    /// </summary>
    public const float Deg2Rad = MathF.PI / 180;

    /// <summary>
    /// A constant factor used to convert an angle measurement from radians to degrees.
    /// </summary>
    public const float Rad2Deg = 180 / MathF.PI;

    /// <summary>
    /// Clamps the given value to the range [0, 1].
    /// </summary>
    /// <param name="value">The value to be clamped.</param>
    /// <returns>The clamped value within the range of 0 to 1.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp01(float value)
    {
        return Math.Clamp(value, 0, 1);
    }
}
