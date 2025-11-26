namespace Guinevere;

/// <summary>
/// Represents the stages of a rendering process in the GUI framework.
/// The rendering process consists of multiple distinct phases that are executed sequentially.
/// </summary>
public enum Pass
{
    /// <summary>
    /// The first pass where the layout tree is built and elements are measured and positioned.
    /// During this phase, UI elements calculate their required dimensions and establish
    /// their positions within the layout hierarchy without performing actual rendering.
    /// </summary>
    Pass1Build,

    /// <summary>
    /// The second pass where the actual rendering of elements occurs.
    /// During this phase, the previously calculated layout information is used to
    /// draw elements to the canvas in their final positions and sizes.
    /// </summary>
    Pass2Render
}
