namespace ToadEngine.Classes.Base.Rendering.Object;

public class Tags
{
    /// <summary>
    /// Gets all existing tags the object has
    /// </summary>
    public readonly List<string> GetTags = new();

    /// <summary>
    /// Adds a tag to the existing object
    /// </summary>
    /// <param name="name"></param>
    public void AddTag(string name)
    {
        if (GetTags.Contains(name)) return;
        GetTags.Add(name);
    }

    /// <summary>
    /// Removes a tag from the existing object
    /// </summary>
    /// <param name="name"></param>
    public void RemoveTag(string name)
    {
        if (!GetTags.Contains(name)) return;
        GetTags.Remove(name);
    }

    /// <summary>
    /// Checks if a GameObject has a tag
    /// </summary>
    /// <param name="tag"></param>
    /// <returns>bool</returns>
    public bool HasTag(string tag) => GetTags.Contains(tag);
}
