using ToadEngine.Classes.Base.Physics;
using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Shaders;
using ToadEngine.Classes.Textures;
using UltraMapper;

namespace ToadEngine.Classes.Base.Rendering.Object;

public struct Textures
{
    public Texture Diffuse;
    public Texture Specular;
    public Texture Normal;
}

public class GameObject
{
    public string? Name = null;

    private List<Behavior?> _behaviors = new();

    public List<IRenderObject> Renderers => Component.GetOfType<IRenderObject>();

    public Component Component = new();
    public Transform Transform = new();

    public PhysicsComponent Physics = new();
    public bool UsePhysics = false;

    public List<GameObject> Children = new();
    public GameObject Parent;
    public bool IsChild;

    public Matrix4 Model;
    public Textures Textures;

    public Matrix4 ProjectionMatrix;
    public Matrix4 ViewMatrix;

    public Scene Scene => Service.Scene;
    public Shader CoreShader => Service.CoreShader;
    public NativeWindow WHandler => Service.NativeWindow;

    public bool IsEnabled = true;

    public GameObject()
    {
        Transform.Front = -Vector3.UnitZ;
        Transform.Up = Vector3.UnitY;
        Transform.Right = Vector3.UnitX;

        Transform.LocalScale = new Vector3(1f);
        Transform.Rotation = new Vector3(0f);
    }

    public T AddComponent<T>() where T : new() => Component.Add<T>();
    public T AddComponent<T>(string name) where T : new() => Component.Add<T>(name);
    public void AddComponent(string name, object obj) => Component.Add(name, obj);
    public void AddComponent(object obj) => Component.Add(obj);

    public T? GetComponent<T>() where T : class => Component.Get<T>();
    public T GetComponent<T>(string name) where T : class => Component.Get<T>(name);
    public List<T> GetComponentsOfType<T>(string name) where T : class => Component.GetOfType<T>();
    public List<object> Components => Component.Components;

    public virtual void Setup() { }

    public void UpdateModelMatrix()
    {
        var localQuat = Quaternion.FromEulerAngles(
            MathHelper.DegreesToRadians(Transform.LocalRotation.X),
            MathHelper.DegreesToRadians(Transform.LocalRotation.Y),
            MathHelper.DegreesToRadians(Transform.LocalRotation.Z));

        var worldQuat = Quaternion.FromEulerAngles(
            MathHelper.DegreesToRadians(Transform.Rotation.X),
            MathHelper.DegreesToRadians(Transform.Rotation.Y),
            MathHelper.DegreesToRadians(Transform.Rotation.Z));

        var finalRotation = worldQuat * localQuat;

        var rotatedLocalPos = Vector3.TransformPosition(Transform.LocalPosition, Matrix4.CreateFromQuaternion(worldQuat));
        var finalPosition = Transform.Position + rotatedLocalPos;

        Model = Matrix4.CreateScale(Transform.Scale) *
                         Matrix4.CreateFromQuaternion(finalRotation) *
                         Matrix4.CreateTranslation(finalPosition);
    }

    public void UpdateWorldTransform()
    {
        if (!IsChild)
            Transform.SetScale(Transform.LocalScale);

        foreach (var child in Children)
        {
            child.IsChild = true;
            child.Parent = this;

            var parentRot = Quaternion.FromEulerAngles(
                MathHelper.DegreesToRadians(Transform.Rotation.X),
                MathHelper.DegreesToRadians(Transform.Rotation.Y),
                MathHelper.DegreesToRadians(Transform.Rotation.Z));

            var rotatedOffset = Vector3.Transform(child.Transform.LocalPosition, parentRot);

            child.Transform.Position = Transform.Position + rotatedOffset;
            child.Transform.Rotation = Transform.Rotation + child.Transform.LocalRotation;
            child.Transform.SetScale(Transform.LocalScale * child.Transform.LocalScale);
        }
    }

    public void AddChild(GameObject child)
    {
        if (Children.Contains(child)) return;
        Children.Add(child);
        UpdateWorldTransform();
    }

    public void RemoveChild(GameObject child)
    {
        if (Children.Contains(child)) return;
        Children.Remove(child);
        UpdateWorldTransform();
    }

    public void Enable()
    {
        IsEnabled = true;
        Scene.Instantiate(this);
    }

    public void Disable()
    {
        IsEnabled = false;
        Scene.DestroyObject(this);
    }

    public void SetupBehaviors()
    {
        _behaviors = Component.GetOfType<Behavior>()!;
        if (_behaviors.Count <= 0) return;

        foreach (var behavior in _behaviors.OfType<Behavior>())
        {
            behavior.GameObject = this;
            GUI.GuiCallBack += behavior.OnGUI;
            behavior.OnStart();
        }
    }

    public void UpdateBehaviors()
    {
        foreach (var behavior in _behaviors.OfType<Behavior>())
        {
            behavior.GameObject = this;
            behavior.OnUpdate();
        }
    }

    public void UpdateBehaviorsFixedTime()
    {
        foreach (var behavior in _behaviors.OfType<Behavior>())
        {
            behavior.GameObject = this;
            behavior.OnFixedUpdate();
        }
    }

    public void CleanupBehaviors()
    {
        foreach (var behavior in _behaviors.OfType<Behavior>())
        {
            foreach (var source in behavior.Sources)
                source.Value.Dispose();

            behavior.OnDispose();
        }
    }

    public void ResizeBehaviors(FramebufferResizeEventArgs e)
    {
        foreach (var behavior in _behaviors.OfType<Behavior>())
        {
            behavior.OnResize(e);
        }
    }

    public GameObject Clone()
    {
        var uMap = new Mapper();
        return uMap.Map<GameObject>(this);
    }
}
