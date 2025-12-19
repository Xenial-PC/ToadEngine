using Prowl.Echo;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Rendering.Object;

public class ComponentManager : ISerializable
{
    [SerializeField] private Dictionary<string, object> _components = new();
    private readonly List<MonoBehavior> _pendingAwake = new();
    private readonly List<MonoBehavior> _pendingStart = new();

    private int _index;

    public void Add(string name, object obj) => _components.TryAdd(name, obj);
    public void Add(object obj) => _components.TryAdd($"component_{_index++}", obj);
    
    public T Add<T>(GameObject go) where T : new()
    {
        var component = new T();

        _components.TryAdd($"component_{_index++}", component);
        if (component is not MonoBehavior behavior) return component;

        RegisterComponent(go, behavior);
        return component;
    }

    public void RegisterComponent(GameObject go, MonoBehavior behavior)
    {
        go.RegisterBehavior(behavior);
        NotifyAdded(behavior);
    }

    public T Add<T>(string name, GameObject go) where T : new()
    {
        var component = new T();
        
        _components.TryAdd(name, component);
        if (component is not MonoBehavior behavior) return component;

        go.RegisterBehavior(behavior);
        NotifyAdded(behavior);
        return component;
    }

    public T Get<T>(string name) where T : class => (_components[name] as T)!;
    public T? Get<T>() where T : class => _components.Select(t => t.Value).OfType<T>().FirstOrDefault();
    public List<T> GetOfType<T>() where T : class => _components.Select(t => t.Value).OfType<T>().ToList();
    public List<object> Components => _components.Select(t => t.Value).ToList();
    public List<MonoBehavior> MonoComponents => _components.Select(t => t.Value as MonoBehavior).ToList()!;

    public List<MonoBehavior> GetComponents(GameObject obj)
    {
        var components = new List<MonoBehavior>();
        if (obj.HasChildren) components.AddRange(obj.Children.SelectMany(child => child.Component.GetOfType<MonoBehavior>()));
        if (obj.IsChild) components.AddRange(obj.Parent.Component.GetOfType<MonoBehavior>());
        components.AddRange(obj.Component.GetOfType<MonoBehavior>());
        return components;
    }

    private void NotifyAdded(MonoBehavior monoBehavior)
    {
        _pendingAwake.Add(monoBehavior);
        _pendingStart.Add(monoBehavior);
    }

    public void FinalizeComponents()
    {
        while (_pendingAwake.Count > 0)
        {
            var toProcess = new List<MonoBehavior>(_pendingAwake);
            _pendingAwake.Clear();

            foreach (var component in toProcess) component.AwakeMethod?.Invoke();
        }

        while (_pendingStart.Count > 0)
        {
            var toProcess = new List<MonoBehavior>(_pendingStart);
            _pendingStart.Clear();

            foreach (var component in toProcess) component.StartMethod?.Invoke();
        }
    }

    public void Serialize(ref EchoObject compound, SerializationContext ctx)
    {
        var components = Serializer.Serialize(_components, ctx);
        var index = Serializer.Serialize(_index, ctx);

        compound.Add("Components", components);
        compound.Add("Index", index);
    }

    public void Deserialize(EchoObject value, SerializationContext ctx)
    {
        var componentObject = value.Get("Components");
        var componentsList = Serializer.Deserialize<Dictionary<string, object>>(componentObject, ctx);

        _components = componentsList!;

        foreach (var component in _components.Where(obj => obj.Value is MonoBehavior)
                     .Select(obj => obj.Value as MonoBehavior).ToList())
        {
            RegisterComponent(component!.GameObject, component);
            Console.WriteLine("Registered Component");
        }
    }
}
