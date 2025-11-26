namespace Guinevere;

/// <summary>
/// Represents a unit value with an associated unit type.
/// Provides functionality to work with different types of units such as pixels, percentages, ratios, etc.,
/// and allows easy conversion and arithmetic operations between units.
/// </summary>
public readonly struct UnitValue(UnitType mode, float value)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitValue"/> struct with a pixel value.
    /// This constructor creates a unit value with <see cref="UnitType.Pixels"/> mode.
    /// </summary>
    /// <param name="value">The pixel value to assign to this unit.</param>
    public UnitValue(float value) : this(UnitType.Pixels, value)
    {
    }

    /// <summary>
    /// Gets the unit type associated with this instance of <see cref="UnitValue"/>.
    /// Represents the mode of measurement or interpretation for the value,
    /// such as pixels, percentages, ratios, or other defined <see cref="UnitType"/> values.
    /// </summary>
    public UnitType Mode { get; } = mode;

    /// <summary>
    /// Gets the numeric value associated with this unit.
    /// The interpretation of this value depends on the <see cref="Mode"/> property.
    /// For example, if Mode is Pixels, this represents pixel units; if Mode is Percentage, this represents a percentage value.
    /// </summary>
    public float Value { get; } = value;

    /// <summary>
    /// Creates a unit value that automatically fits content with an optional scaling percentage.
    /// This unit type adjusts its size based on the content it contains.
    /// </summary>
    /// <param name="percentage">The scaling percentage to apply. Default is 1 (100%).</param>
    /// <returns>A new <see cref="UnitValue"/> with <see cref="UnitType.Auto"/> mode.</returns>
    public static UnitValue FitContent(float percentage = 1)
    {
        return new UnitValue(UnitType.Auto, percentage);
    }

    /// <summary>
    /// Creates a unit value that expands to fill available space with an optional scaling percentage.
    /// This unit type takes up remaining space in its container.
    /// </summary>
    /// <param name="percentage">The scaling percentage to apply. Default is 1 (100%).</param>
    /// <returns>A new <see cref="UnitValue"/> with <see cref="UnitType.Expand"/> mode.</returns>
    public static UnitValue Expand(float percentage = 1)
    {
        return new UnitValue(UnitType.Expand, percentage);
    }

    /// <summary>
    /// Creates a unit value based on a ratio relative to other elements.
    /// This unit type is proportional to other ratio-based units in the same context.
    /// </summary>
    /// <param name="ratio">The ratio value. Higher values take proportionally more space.</param>
    /// <returns>A new <see cref="UnitValue"/> with <see cref="UnitType.Ratio"/> mode.</returns>
    public static UnitValue Ratio(float ratio)
    {
        return new UnitValue(UnitType.Ratio, ratio);
    }

    /// <summary>
    /// Creates a unit value based on a percentage of the parent container's size.
    /// </summary>
    /// <param name="percentage">The percentage value (e.g., 0.5 for 50%, 1.0 for 100%).</param>
    /// <returns>A new <see cref="UnitValue"/> with <see cref="UnitType.Percentage"/> mode.</returns>
    public static UnitValue Percentage(float percentage)
    {
        return new UnitValue(UnitType.Percentage, percentage);
    }

    /// <summary>
    /// Creates a unit value with an absolute pixel measurement.
    /// </summary>
    /// <param name="pixels">The number of pixels.</param>
    /// <returns>A new <see cref="UnitValue"/> with <see cref="UnitType.Pixels"/> mode.</returns>
    public static UnitValue Pixels(float pixels)
    {
        return new UnitValue(UnitType.Pixels, pixels);
    }

    /// <summary>
    /// Adds an integer value to the unit value, preserving the original unit type.
    /// </summary>
    /// <param name="unitValue">The unit value to add to.</param>
    /// <param name="value">The integer value to add.</param>
    /// <returns>A new <see cref="UnitValue"/> with the same mode and the sum of the values.</returns>
    public static UnitValue operator +(UnitValue unitValue, int value) => new(unitValue.Mode, unitValue.Value + value);

    /// <summary>
    /// Implicitly converts an integer to a pixel-based unit value.
    /// </summary>
    /// <param name="pixels">The pixel value to convert.</param>
    /// <returns>A new <see cref="UnitValue"/> with <see cref="UnitType.Pixels"/> mode.</returns>
    public static implicit operator UnitValue(int pixels) => Pixels(pixels);

    /// <summary>
    /// Implicitly converts a float to a pixel-based unit value.
    /// </summary>
    /// <param name="pixels">The pixel value to convert.</param>
    /// <returns>A new <see cref="UnitValue"/> with <see cref="UnitType.Pixels"/> mode.</returns>
    public static implicit operator UnitValue(float pixels) => Pixels(pixels);

    /// <summary>
    /// Implicitly converts a double to a pixel-based unit value.
    /// </summary>
    /// <param name="pixels">The pixel value to convert.</param>
    /// <returns>A new <see cref="UnitValue"/> with <see cref="UnitType.Pixels"/> mode.</returns>
    public static implicit operator UnitValue(double pixels) => Pixels((float)pixels);

    /// <summary>
    /// Implicitly converts a unit value to an integer by extracting its numeric value.
    /// Note: This conversion loses unit type information and may involve precision loss.
    /// </summary>
    /// <param name="unitValue">The unit value to convert.</param>
    /// <returns>The numeric value as an integer.</returns>
    public static implicit operator int(UnitValue unitValue) => (int)unitValue.Value;

    /// <summary>
    /// Implicitly converts a unit value to a float by extracting its numeric value.
    /// Note: This conversion loses unit type information.
    /// </summary>
    /// <param name="unitValue">The unit value to convert.</param>
    /// <returns>The numeric value as a float.</returns>
    public static implicit operator float(UnitValue unitValue) => unitValue.Value;

    /// <summary>
    /// Implicitly converts a unit value to a double by extracting its numeric value.
    /// Note: This conversion loses unit type information.
    /// </summary>
    /// <param name="unitValue">The unit value to convert.</param>
    /// <returns>The numeric value as a double.</returns>
    public static implicit operator double(UnitValue unitValue) => unitValue.Value;

    /// <summary>
    /// Adds a float value to the unit value, preserving the original unit type.
    /// </summary>
    /// <param name="unitValue">The unit value to add to.</param>
    /// <param name="value">The float value to add.</param>
    /// <returns>A new <see cref="UnitValue"/> with the same mode and the sum of the values.</returns>
    public static UnitValue operator +(UnitValue unitValue, float value) =>
        new(unitValue.Mode, unitValue.Value + value);

    /// <summary>
    /// Adds a double value to the unit value, preserving the original unit type.
    /// </summary>
    /// <param name="unitValue">The unit value to add to.</param>
    /// <param name="value">The double value to add.</param>
    /// <returns>A new <see cref="UnitValue"/> with the same mode and the sum of the values.</returns>
    public static UnitValue operator +(UnitValue unitValue, double value) =>
        new(unitValue.Mode, unitValue.Value + (float)value);

    /// <summary>
    /// Adds an integer value to the unit value, preserving the original unit type.
    /// </summary>
    /// <param name="value">The integer value to add.</param>
    /// <param name="unitValue">The unit value to add to.</param>
    /// <returns>A new <see cref="UnitValue"/> with the same mode and the sum of the values.</returns>
    public static UnitValue operator +(int value, UnitValue unitValue) => new(unitValue.Mode, unitValue.Value + value);

    /// <summary>
    /// Adds a float value to the unit value, preserving the original unit type.
    /// </summary>
    /// <param name="value">The float value to add.</param>
    /// <param name="unitValue">The unit value to add to.</param>
    /// <returns>A new <see cref="UnitValue"/> with the same mode and the sum of the values.</returns>
    public static UnitValue operator +(float value, UnitValue unitValue) =>
        new(unitValue.Mode, unitValue.Value + value);

    /// <summary>
    /// Adds a double value to the unit value, preserving the original unit type.
    /// </summary>
    /// <param name="value">The double value to add.</param>
    /// <param name="unitValue">The unit value to add to.</param>
    /// <returns>A new <see cref="UnitValue"/> with the same mode and the sum of the values.</returns>
    public static UnitValue operator +(double value, UnitValue unitValue) =>
        new(unitValue.Mode, unitValue.Value + (float)value);
}
