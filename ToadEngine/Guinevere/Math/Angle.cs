namespace Guinevere;

/// <summary>
/// Represents an angle defined in terms of radians, and provides methods for converting and performing operations with angles.
/// </summary>
public readonly struct Angle
{
    /// <summary>
    /// Represents an angle defined in terms of radians, and provides methods for converting and performing operations with angles.
    /// </summary>
    public Angle(float radians)
    {
        Radian = radians;
    }

    /// <summary>
    /// Represents the angle in radians.
    /// </summary>
    public float Radian { get; }

    /// <summary>
    /// Represents the angle in degrees, derived from the angle's radian value.
    /// </summary>

    public float Degree => Radian * ImMath.Rad2Deg;

    /// <summary>
    /// Represents a full rotation angle, equivalent to 360 degrees or 2π radians.
    /// </summary>
    public static Angle FullCircle => new(MathF.PI * 2f);

    /// <summary>
    /// Creates a new <see cref="Angle"/> instance representing the specified number of turns.
    /// </summary>
    /// <param name="turns">The angle value in turns, where 1 turn is equivalent to 360 degrees or 2π radians.</param>
    /// <returns>A new <see cref="Angle"/> corresponding to the specified number of turns.</returns>
    public static Angle Turns(float turns)
    {
        return new Angle(turns * MathF.PI * 2f);
    }

    /// <summary>
    /// Creates a new <see cref="Angle"/> instance from the given degree value.
    /// </summary>
    /// <param name="degrees">The angle value in degrees.</param>
    /// <returns>A new <see cref="Angle"/> representing the specified degree value.</returns>
    public static Angle Degrees(float degrees)
    {
        return new Angle(degrees * ImMath.Deg2Rad);
    }

    /// <summary>
    /// Creates a new <see cref="Angle"/> instance from the given radian value.
    /// </summary>
    /// <param name="radians">The angle value in radians.</param>
    /// <returns>A new <see cref="Angle"/> representing the specified radian value.</returns>
    public static Angle Radians(float radians)
    {
        return new Angle(radians);
    }

    /// <summary>
    /// Calculates the direction vector corresponding to the angle represented by this <see cref="Angle"/> instance.
    /// </summary>
    /// <returns>A <see cref="Vector2"/> representing the direction of the angle, where the X and Y components
    /// correspond to the cosine and sine of the angle in radians, respectively.</returns>
    public Vector2 GetDirectionVector()
    {
        return new Vector2(MathF.Cos(Radian), MathF.Sin(Radian));
    }

    /// <summary>
    /// Defines addition operator to combine two <see cref="Angle"/> instances by summing their radian values.
    /// </summary>
    /// <param name="a">The first <see cref="Angle"/> to add.</param>
    /// <param name="b">The second <see cref="Angle"/> to add.</param>
    /// <returns>A new <see cref="Angle"/> representing the sum of the two angles.</returns>
    public static Angle operator +(Angle a, Angle b) => new(a.Radian + b.Radian);

    /// <summary>
    /// Defines a subtraction operator for the <see cref="Angle"/> struct that subtracts one angle from another.
    /// </summary>
    /// <param name="a">The first angle in the subtraction operation.</param>
    /// <param name="b">The second angle to subtract from the first angle.</param>
    /// <returns>A new <see cref="Angle"/> representing the result of subtracting <paramref name="b"/> from <paramref name="a"/>.</returns>
    public static Angle operator -(Angle a, Angle b) => new(a.Radian - b.Radian);

    /// <summary>
    /// Defines an implicit conversion from a <see cref="float"/> representing radians to an <see cref="Angle"/> instance.
    /// </summary>
    /// <param name="radians">The angle in radians to be converted to an <see cref="Angle"/>.</param>
    /// <returns>An <see cref="Angle"/> instance representing the given angle in radians.</returns>
    public static implicit operator Angle(float radians) => new(radians);

    /// <summary>
    /// Defines an explicit conversion operator for converting an <see cref="Angle"/> to a <see cref="float"/>,
    /// returning the radian value of the angle.
    /// </summary>
    /// <param name="angle">The <see cref="Angle"/> to be converted to radians.</param>
    /// <returns>The radian value of the <see cref="Angle"/> as a <see cref="float"/>.</returns>
    public static implicit operator float(Angle angle) => angle.Radian;
}
