using System.Reflection;
using System.Runtime.CompilerServices;
using ToadEngine.Classes.Base.Audio;
using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Physics;
using ToadEngine.Classes.Base.UI;
using ToadEngine.Classes.Shaders;
using ToadEngine.Classes.Textures;
using static ToadEngine.Classes.Base.Rendering.GameObject.Structs;

namespace ToadEngine.Classes.Base.Rendering;

public class GameObject : RenderObject
{
    private readonly Dictionary<string, object> _components = new();
    private int _index;
    public string? Name = null;

    public Structs.GameObj Obj = new();
    private List<Behavior?> _behaviours = new();
    public PhysicsComponent Physics = new();
    public bool UsePhysics = false;

    public List<GameObject> Children = new();
    public GameObject Parent;
    public bool IsChild;

    public Structs.Transform Transform = new();

    public Scene Scene => Service.Scene;
    public Shader GetCoreShader() => Service.CoreShader;

    public GameObject(string? name = null)
    {
        Name = name;

        Transform.Front = -Vector3.UnitZ;
        Transform.Up = Vector3.UnitY;
        Transform.Right = Vector3.UnitX;

        Transform.LocalScale = new Vector3(1f);
        Transform.Rotation = new Vector3(0f);
    }

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

        Obj.Model = Matrix4.CreateScale(Transform.Scale) *
                         Matrix4.CreateFromQuaternion(finalRotation) *
                         Matrix4.CreateTranslation(finalPosition);
    }

    public void AddComponent(string name, object obj)
    {
        _components.TryAdd(name, obj);
    }

    public void AddComponent(object obj)
    {
        _components.TryAdd($"component_{_index++}", obj);
    }

    public T AddComponent<T>() where T : new()
    {
        var component = new T();
        _components.TryAdd($"component_{_index++}", component);
        return component;
    }

    public T GetComponent<T>(string name) where T : class
    {
        return (_components[name] as T)!;
    }

    public T? GetComponent<T>() where T : class
    {
        return _components.Select(t => t.Value).OfType<T>().FirstOrDefault();
    }

    public List<T> GetComponents<T>() where T : class
    {
        return _components.Select(t => t.Value).OfType<T>().ToList();
    }

    public List<object> GetComponents()
    {
        return _components.Select(t => t.Value).ToList();
    }

    public void TransformPosition(Vector3 position)
    {
        Transform.LocalPosition = position;
    }

    public void TransformRotation(Vector3 rotation)
    {
        Transform.LocalRotation = rotation;
    }

    public void TransformSize(Vector3 size)
    {
        Transform.LocalScale = size;
    }

    public void AddChild(GameObject child)
    {
        if (Children.Contains(child)) return;
        Children.Add(child);
    }

    public void RemoveChild(GameObject child)
    {
        if (Children.Contains(child)) return;
        Children.Remove(child);
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

    public void SetupBehaviours()
    {
        _behaviours = GetComponents<Behavior>()!;
        if (_behaviours.Count <= 0) return;

        foreach (var behaviour in _behaviours.OfType<Behavior>())
        {
            behaviour.GameObject = this;
            behaviour.GameObject.Obj = Obj;
            GUI.GuiCallBack += behaviour.OnGUI;
            behaviour.Setup();
        }
    }

    public void UpdateBehaviours(float deltaTime)
    {
        foreach (var behaviour in _behaviours.OfType<Behavior>())
        {
            Obj = behaviour.GameObject.Obj;
            behaviour.GameObject = this;

            behaviour.DeltaTime = deltaTime;
            behaviour.Update(deltaTime);
        }
    }

    public void CleanupBehaviours()
    {
        foreach (var behaviour in _behaviours.OfType<Behavior>())
        {
            foreach (var source in behaviour.Sources)
                source.Value.Dispose();

            behaviour.Dispose();
        }
    }

    public void ResizeBehaviours(FramebufferResizeEventArgs e)
    {
        foreach (var behaviour in _behaviours.OfType<Behavior>())
            behaviour.Resize(e);
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

    public class Structs
    {
        public class Transform
        {
            public Vector3 Position = Vector3.Zero;
            public Vector3 Rotation = Vector3.Zero;
            public Vector3 Scale { get; private set; } = Vector3.Zero;

            public Vector3 LocalPosition = Vector3.Zero;
            public Vector3 LocalRotation = Vector3.Zero;
            public Vector3 LocalScale = Vector3.Zero;

            public Vector3 Front = Vector3.Zero;
            public Vector3 Up = Vector3.Zero;
            public Vector3 Right = Vector3.Zero;

            public void SetScale(Vector3 scale)
            {
                Scale = scale;
            }
        }

        public struct GameObj
        {
            public Matrix4 Model;
            public Textures Texture;

            public Matrix4 ProjectionMatrix;
            public Matrix4 ViewMatrix;

            public struct Textures
            {
                public Texture Diffuse;
                public Texture Specular;
            }
        }
    }
}
