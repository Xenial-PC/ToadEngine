namespace Guinevere;

/// <summary>
/// Represents a drawable object that can be rendered onto a canvas.
/// </summary>
public interface IDrawable
{
    /// <summary>
    /// Gets or sets the paint properties associated with the drawable entity.
    /// This property defines the visual style, such as color, stroke, and other
    /// painting characteristics, used when rendering the element.
    /// </summary>
    SKPaint? Paint { get; }

    /// <summary>
    /// Renders the drawable element onto the specified canvas according to the provided GUI and layout node contexts.
    /// </summary>
    /// <param name="gui">The GUI instance providing rendering context and state.</param>
    /// <param name="node">The layout node containing positional and styling data for rendering.</param>
    /// <param name="canvas">The canvas on which the drawable element will be rendered.</param>
    void Render(Gui gui, LayoutNode node, SKCanvas canvas);
}
