using ToadEngine.Classes.Base.Physics.Managers;
using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Shaders;

namespace ToadEngine.Classes.Base.Rendering.Object;

public class GameObject
{
    public readonly Guid Guid = Guid.NewGuid();

    public string? Name = null;

    public List<IRenderObject> Renderers => Component.GetOfType<IRenderObject>();

    public ComponentManager Component = new();

    public Transform Transform
    {
        get
        {
            var transform = GetComponent<Transform>();
            if (transform != null) return transform;

            transform = AddComponent<Transform>();
            transform.Front = -Vector3.UnitZ;
            transform.Up = Vector3.UnitY;
            transform.Right = Vector3.UnitX;

            transform.LocalScale = new Vector3(1f);
            transform.Rotation = new Vector3(0f);

            return transform;
        }
    }

    public bool UsePhysics = false;

    public List<GameObject> Children = new();
    public GameObject Parent = null!;
    public bool IsChild;
    public bool HasChildren => Children.Count > 0;

    public Matrix4 Model;
    
    public Scene Scene => Service.Scene;
    public Shader CoreShader => Service.CoreShader;
    public NativeWindow WHandler => Service.NativeWindow;

    public bool IsEnabled = true;

    public T AddComponent<T>() where T : new() => Component.Add<T>(this);
    public T AddComponent<T>(string name) where T : new() => Component.Add<T>(name, this);
    public void AddComponent(string name, object obj) => Component.Add(name, obj);
    public void AddComponent(object obj) => Component.Add(obj);

    public T? GetComponent<T>() where T : class => Component.Get<T>();
    public T GetComponent<T>(string name) where T : class => Component.Get<T>(name);
    public List<T> GetComponentsOfType<T>(string name) where T : class => Component.GetOfType<T>();
    public List<object> Components => Component.Components;

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

    public void RegisterBehavior(MonoBehavior monoBehavior)
    {
        MethodRegistry.RegisterMethods(monoBehavior);

        monoBehavior.GameObject = this;
        GUI.GuiCallBack += monoBehavior.OnGuiMethod;
    }

    public void UpdateBehaviors()
    {
        foreach (var behavior in Components.OfType<MonoBehavior>())
        {
            behavior.GameObject = this;
            behavior.UpdateMethod?.Invoke();
        }
    }

    public void UpdateBehaviorsFixedTime()
    {
        foreach (var behavior in Components.OfType<MonoBehavior>())
        {
            behavior.GameObject = this;
            behavior.FixedUpdateMethod?.Invoke();
        }
    }

    public void CleanupBehaviors()
    {
        foreach (var behavior in Components.OfType<MonoBehavior>())
        {
            foreach (var source in behavior.Sources)
                source.Value.Dispose();

            behavior.DisposeMethod?.Invoke();
        }
    }

    public void ResizeBehaviors(FramebufferResizeEventArgs e)
    {
        foreach (var behavior in Components.OfType<MonoBehavior>())
            behavior.OnResizeMethod?.Invoke(e);
    }

    public static void SetupTriggers()
    {
        TriggerManager.OnEnter += (a, b) =>
        {
            if (!MonoBehavior.BodyToGameObject.TryGetValue(a, out var objA) ||
                !MonoBehavior.BodyToGameObject.TryGetValue(b, out var objB)) return;

            foreach (var component in objA.Component.GetComponents(objA))
                component.OnTriggerEnterMethod?.Invoke(objB);

            foreach (var component in objB.Component.GetComponents(objB))
                component.OnTriggerEnterMethod?.Invoke(objA);
        };

        TriggerManager.OnExit += (a, b) =>
        {
            if (!MonoBehavior.BodyToGameObject.TryGetValue(a, out var objA) ||
                !MonoBehavior.BodyToGameObject.TryGetValue(b, out var objB)) return;

            foreach (var component in objA.Component.GetComponents(objA))
                component.OnTriggerExitMethod?.Invoke(objB);

            foreach (var component in objB.Component.GetComponents(objB))
                component.OnTriggerExitMethod?.Invoke(objA);
        };
    }
}
