namespace Guinevere;

/// <summary>
/// Provides methods to manage the entry and exit of layout node contexts within a UI rendering or layout system.
/// </summary>
public interface ILayoutNodeEnterExit
{
    /// <summary>
    /// Enters the specified layout node's scope and begins managing its state or properties.
    /// </summary>
    /// <param name="node">
    /// The layout node to enter and manage within a new scope.
    /// </param>
    /// <returns>
    /// A <see cref="LayoutNodeScope"/> representing the entered scope of the given layout node.
    /// </returns>
    LayoutNodeScope Enter(LayoutNode node);

    /// <summary>
    /// Exits the current layout node's scope and returns the parent layout node, if any.
    /// </summary>
    /// <returns>
    /// The parent layout node after exiting the current scope.
    /// </returns>
    LayoutNode Exit();
}
