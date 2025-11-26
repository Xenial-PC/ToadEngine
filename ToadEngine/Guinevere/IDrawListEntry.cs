namespace Guinevere;

/// <summary>
/// Represents an entry in a drawing list, allowing customizable drawing actions
/// to be performed on a given GUI layout node using an associated canvas.
/// </summary>
public interface IDrawListEntry
{
    /// <summary>
    /// Executes a drawing operation using the provided GUI, layout node, and canvas.
    /// </summary>
    /// <param name="gui">The GUI context to utilize for the drawing operation.</param>
    /// <param name="node">The layout node in the GUI hierarchy where the operation should take place.</param>
    /// <param name="canvas">The canvas on which the drawing operation is performed.</param>
    void Execute(Gui gui, LayoutNode node, SKCanvas canvas);
}
