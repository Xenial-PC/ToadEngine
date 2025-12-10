using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Rendering.Object;

public class Component
{
    private readonly Dictionary<string, object> _components = new();
    private readonly List<Behavior> _pendingAwake = new();
    private readonly List<Behavior> _pendingStart = new();

    private int _index;

    public void Add(string name, object obj) => _components.TryAdd(name, obj);
    public void Add(object obj) => _components.TryAdd($"component_{_index++}", obj);
    
    public T Add<T>(GameObject go) where T : new()
    {
        var component = new T();

        _components.TryAdd($"component_{_index++}", component);
        if (component is not Behavior behavior) return component;

        go.RegisterBehavior(behavior);
        NotifyAdded(behavior);
        return component;
    }

    public T Add<T>(string name, GameObject go) where T : new()
    {
        var component = new T();

        _components.TryAdd(name, component);
        if (component is not Behavior behavior) return component;

        go.RegisterBehavior(behavior);
        NotifyAdded(behavior);
        return component;
    }

    public T Get<T>(string name) where T : class => (_components[name] as T)!;
    public T? Get<T>() where T : class => _components.Select(t => t.Value).OfType<T>().FirstOrDefault();
    public List<T> GetOfType<T>() where T : class => _components.Select(t => t.Value).OfType<T>().ToList();
    public List<object> Components => _components.Select(t => t.Value).ToList();

    public List<Behavior> GetComponents(GameObject obj)
    {
        var components = new List<Behavior>();
        if (obj.HasChildren) components.AddRange(obj.Children.SelectMany(child => child.Component.GetOfType<Behavior>()));
        if (obj.IsChild) components.AddRange(obj.Parent.Component.GetOfType<Behavior>());
        components.AddRange(obj.Component.GetOfType<Behavior>());
        return components;
    }

    private void NotifyAdded(Behavior behavior)
    {
        _pendingAwake.Add(behavior);
        _pendingStart.Add(behavior);
    }

    public void FinalizeComponents()
    {
        while (_pendingAwake.Count > 0)
        {
            var toProcess = new List<Behavior>(_pendingAwake);
            _pendingAwake.Clear();

            foreach (var component in toProcess) component.AwakeMethod?.Invoke();
        }

        while (_pendingStart.Count > 0)
        {
            var toProcess = new List<Behavior>(_pendingStart);
            _pendingStart.Clear();

            foreach (var component in toProcess) component.StartMethod?.Invoke();
        }
    }
}
