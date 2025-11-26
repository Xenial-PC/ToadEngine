namespace Guinevere;

/// <summary>
/// Represents a structure that defines an edge or border with measurements for each side: top, right, bottom, and left.
/// </summary>
public readonly struct Edge(float top, float right, float bottom, float left)
{
    /// <summary>
    /// Gets a value representing the measurement of the left edge.
    /// </summary>
    public float Left { get; init; } = left;

    /// <summary>
    /// Gets a value representing the measurement of the top edge.
    /// </summary>
    public float Top { get; init; } = top;

    /// <summary>
    /// Gets a value representing the measurement of the right edge.
    /// </summary>
    public float Right { get; init; } = right;

    /// <summary>
    /// Gets a value representing the measurement of the bottom edge.
    /// </summary>
    public float Bottom { get; init; } = bottom;

    /// <summary>
    /// Gets a value representing the combined measurement of the left and right edges.
    /// </summary>
    public float X => Left + Right;

    /// <summary>
    /// Gets a value representing the combination of the top and bottom edge measurements.
    /// </summary>
    public float Y => Top + Bottom;

    /// <summary>
    /// Gets a <see cref="Vector2"/> representing the combination of the top and left edge measurements.
    /// </summary>
    public Vector2 TopLeft => new(Top, Left);

    /// <summary>
    /// Gets a <see cref="Vector2"/> representing the combination of the bottom and right edge measurements.
    /// </summary>
    public Vector2 BottomRight => new(Bottom, Right);

    /// <summary>
    /// Represents a structure that defines an edge or border with measurements for each side: top, right, bottom, and left.
    /// </summary>
    public Edge(float all)
        : this(all, all, all, all)
    {
    }

    /// <summary>
    /// Represents a structure that defines an edge or border with measurements for each side: top, right, bottom, and left.
    /// </summary>
    public Edge(float x, float y)
        : this(x, y, x, y)
    {
    }

    /// <summary>
    /// Adds the corresponding sides of two Edge instances.
    /// </summary>
    /// <param name="a">The first Edge instance.</param>
    /// <param name="b">The second Edge instance.</param>
    /// <returns>A new Edge instance representing the sum of the corresponding sides.</returns>
    public static Edge operator +(Edge a, Edge b) =>
        new(a.Top + b.Top, a.Right + b.Right, a.Bottom + b.Bottom, a.Left + b.Left);

    /// <summary>
    /// Subtracts the corresponding sides of one Edge instance from another.
    /// </summary>
    /// <param name="a">The first Edge instance.</param>
    /// <param name="b">The second Edge instance whose values will be subtracted from the first.</param>
    /// <returns>A new Edge instance representing the difference of the corresponding sides.</returns>
    public static Edge operator -(Edge a, Edge b) =>
        new(a.Top - b.Top, a.Right - b.Right, a.Bottom - b.Bottom, a.Left - b.Left);
}
