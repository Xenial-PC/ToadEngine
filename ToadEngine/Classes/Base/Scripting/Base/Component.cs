using Prowl.Echo;
using Prowl.PaperUI;
using ToadEngine.Classes.Base.Audio;
using ToadEngine.Classes.Base.Physics;
using ToadEngine.Classes.Base.Raycasting;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Shaders;

namespace ToadEngine.Classes.Base.Scripting.Base;

public abstract class Component
{
    public readonly Guid Guid = Guid.NewGuid();

    public RaycastManager Raycast = new(Service.Physics.BufferPool);
    public static Dictionary<int, GameObject> BodyToGameObject = new();

    public GameObject GameObject = null!;

    public Transform Transform => GameObject.Transform;
    public Scene Scene => Service.Scene;
    public NativeWindow WHandler => Service.NativeWindow;

    public AudioManager AudioManger => Service.Scene.AudioManager;
    public PhysicsSimulation Physics => Service.Physics;

    public int GetSound(string name) => AudioManger.GetSound(name);
    public Dictionary<string, Source> Sources = new();

    public Source? GetSource(string name) => Sources.GetValueOrDefault(name);

    public Paper UI => GUI.UI;
    public Shader CoreShader => Service.CoreShader;

    public void LoadScene(string name) => Service.Window.LoadScene(name);

    public T AddComponent<T>() where T : new() => GameObject.Component.Add<T>(GameObject);
    public T AddComponent<T>(string name) where T : new() => GameObject.Component.Add<T>(name, GameObject);
    public void AddComponent(string name, GameObject go, MonoBehavior obj) => GameObject.Component.Add(name, go, obj);
    public void AddComponent(object obj) => GameObject.Component.Add(obj);

    public T? GetComponent<T>() where T : class => GameObject.Component.Get<T>();
    public T GetComponent<T>(string name) where T : class => GameObject.Component.Get<T>(name);
    public List<T> GetComponentsOfType<T>(string name) where T : class => GameObject.Component.GetOfType<T>();
    public List<object> Components => GameObject.Component.Components;

    public GameObject? FindGameObject(string name) => Scene.ObjectManager.FindGameObject(name);
    public GameObject? FindGameObjectByTag(string tag) => Scene.ObjectManager.FindGameObjectByTag(tag);
    public List<GameObject>? FindGameObjectsByTag(string tag) => Scene.ObjectManager.FindGameObjectsByTag(tag);

    public List<string> GetTags => GameObject.GetTags;
    public bool HasTag(string name) => GameObject.HasTag(name);
    public void AddTag(string name) => GameObject.Tags.AddTag(name);
    public void RemoveTag(string name) => GameObject.Tags.RemoveTag(name);

    public void Instantiate(GameObject gameObject, InstantiateType type = InstantiateType.Early) =>
        Scene.Instantiate(gameObject, type);

    public void Instantiate(List<GameObject> gameObjects, InstantiateType type = InstantiateType.Early) =>
        Scene.Instantiate(gameObjects, type);

    public void Destroy(GameObject gameObject, InstantiateType type = InstantiateType.Early) =>
        Scene.DestroyObject(gameObject, type);

    public void Destroy(List<GameObject> gameObjects, InstantiateType type = InstantiateType.Early) =>
        Scene.DestroyObject(gameObjects, type);

    public EchoObject Serialize => Serializer.Serialize(this);
}
