namespace ToadEngine.Classes.Base.Rendering.Object;

public class Component
{
    private readonly Dictionary<string, object> _components = new();
    private int _index;

    public void Add(string name, object obj) => _components.TryAdd(name, obj);
    public void Add(object obj) => _components.TryAdd($"component_{_index++}", obj);
    public T Add<T>() where T : new()
    {
        var component = new T();
        _components.TryAdd($"component_{_index++}", component);
        return component;
    }

    public T Get<T>(string name) where T : class => (_components[name] as T)!;
    public T? Get<T>() where T : class => _components.Select(t => t.Value).OfType<T>().FirstOrDefault();
    public List<T> GetOfType<T>() where T : class => _components.Select(t => t.Value).OfType<T>().ToList();
    public List<object> Components => _components.Select(t => t.Value).ToList();
}
