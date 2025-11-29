using ToadEngine.Classes.Base.Physics;
using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Shaders;
using ToadEngine.Classes.Textures;

namespace ToadEngine.Classes.Base.Rendering.Object;

public class GameObject : RenderObject
{
    public string? Name;

    private List<Behavior?> _behaviours = new();

    public Component Component = new();
    public Transform Transform = new();

    public PhysicsComponent Physics = new();
    public bool UsePhysics = false;

    public List<GameObject> Children = new();
    public GameObject Parent;
    public bool IsChild;

    public Scene Scene => Service.Scene;
    public Shader CoreShader => Service.CoreShader;

    public Matrix4 Model;
    public Textures Texture;

    public Matrix4 ProjectionMatrix;
    public Matrix4 ViewMatrix;

    public struct Textures
    {
        public Texture Diffuse;
        public Texture Specular;
    }

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

        Model = Matrix4.CreateScale(Transform.Scale) *
                         Matrix4.CreateFromQuaternion(finalRotation) *
                         Matrix4.CreateTranslation(finalPosition);
    }

    public void TransformPosition(Vector3 position) => Transform.LocalPosition = position;
    public void TransformRotation(Vector3 rotation) => Transform.LocalRotation = rotation;
    public void TransformSize(Vector3 size) => Transform.LocalScale = size;

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
        _behaviours = Component.GetOfType<Behavior>()!;
        if (_behaviours.Count <= 0) return;

        foreach (var behaviour in _behaviours.OfType<Behavior>())
        {
            behaviour.GameObject = this;
            GUI.GuiCallBack += behaviour.OnGUI;
            behaviour.Setup();
        }
    }

    public void UpdateBehaviours(float deltaTime)
    {
        foreach (var behaviour in _behaviours.OfType<Behavior>())
        {
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
}
