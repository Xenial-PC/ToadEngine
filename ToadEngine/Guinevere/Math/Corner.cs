#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Guinevere;

/// <summary>
/// Represents the corners of a shape as a set of flags, allowing multiple corners to be specified.
/// </summary>
/// <remarks>
/// This enum is typically used to define which corners of a shape are affected by transformations
/// such as rounding or other modifications.
/// The individual flag values can be combined using bitwise operations to specify multiple corners.
/// </remarks>
[Flags]
public enum Corner
{
    None = 0,
    TopLeft = 1 << 0,
    TopRight = 1 << 1,
    BottomLeft = 1 << 2,
    BottomRight = 1 << 3,
    Top = TopLeft | TopRight,
    Bottom = BottomLeft | BottomRight,
    Left = TopLeft | BottomLeft,
    Right = BottomRight | TopRight,
    BottomRightAndTopLeft = BottomRight | TopLeft,
    BottomLeftAndTopRight = BottomLeft | TopRight,
    All = TopLeft | TopRight | BottomLeft | BottomRight
}
