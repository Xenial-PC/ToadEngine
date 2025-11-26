#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Guinevere;

/// <summary>
/// Represents the types of user interactions that can be applied to UI elements.
/// </summary>
[Flags]
public enum Interactions
{
    None = 0,
    Hover = 1 << 0,
    Hold = 1 << 1,
    Focus = 1 << 2,
    Click = 1 << 3
}
