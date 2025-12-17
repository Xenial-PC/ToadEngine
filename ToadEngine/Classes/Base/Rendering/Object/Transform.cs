using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Rendering.Object;

public class Transform : MonoBehavior
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
