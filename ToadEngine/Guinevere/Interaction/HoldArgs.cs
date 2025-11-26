namespace Guinevere;

/// <summary>
/// Represents the necessary input information for handling a hold interaction.
/// This structure contains data about the initial and current positions of the pointer
/// during a hold gesture, including the difference between these positions.
/// </summary>
public readonly struct HoldArgs
{
    /// <summary>
    /// Gets the initial position of the object or pointer at the start of an interaction.
    /// Represents the location where the interaction originated, before any movement.
    /// </summary>
    public Vector2 StartPosition { get; init; }

    /// <summary>
    /// Gets the current position of the object or pointer during an interaction.
    /// Represents the updated location relative to the starting position.
    /// </summary>
    public Vector2 CurrentPosition { get; init; }

    /// <summary>
    /// Gets the difference between the current position and the starting position.
    /// Represents the positional offset of an object or input from its initial point.
    /// </summary>
    public Vector2 DeltaPosition => CurrentPosition - StartPosition;
}
