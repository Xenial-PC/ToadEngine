using ToadEngine.Classes.Base.Rendering;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace ToadEngine.Classes.Base.Objects.View;

public class Camera : GameObject
{
    private Vector3 _front = -Vector3.UnitZ;
    private Vector3 _up = Vector3.UnitY;
    private Vector3 _right = Vector3.UnitX;

    private Vector2 _lastPos;

    private float _fov = MathHelper.PiOver2;
    private float _speed = 1.5f;
    private float _sensitivity = 0.1f;

    private bool _firstMove = true;

    public float AspectRatio { get; set; }

    public Vector3 Front
    {
        get => _front;
        set => _front = value;
    }

    public Vector3 Up
    {
        get => _up;
        set => _up = value;
    }

    public Vector3 Right
    {
        get => _right;
        set => _right = value;
    }

    public float Sensitivity
    {
        get => _sensitivity;
        set => _sensitivity = value;
    }

    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set
        {
            var angle = MathHelper.Clamp(value, 1f, 90f);
            _fov = MathHelper.DegreesToRadians(angle);
        }
    }

    public Camera(float aspectRatio)
    {
        Transform.LocalPosition = new Vector3(0, 0, 0);
        AspectRatio = aspectRatio;
    }

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Transform.LocalPosition, Transform.LocalPosition + _front, _up);
    }

    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 1000f);
    }

    public void UpdateVectors()
    {
        var pitch = MathHelper.DegreesToRadians(Transform.LocalRotation.X);
        var yaw = MathHelper.DegreesToRadians(Transform.LocalRotation.Y);
        var roll = MathHelper.DegreesToRadians(Transform.LocalRotation.Z);

        _front.X = MathF.Cos(pitch) * MathF.Cos(yaw);
        _front.Y = MathF.Sin(pitch);
        _front.Z = MathF.Cos(pitch) * MathF.Sin(yaw);

        _front = Vector3.Normalize(_front);

        _right = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, _front));
        _up = Vector3.Normalize(Vector3.Cross(_front, _right));

        if (roll == 0) return;
        var rollMatrix = Matrix3.CreateFromAxisAngle(_front, roll);
        _right = Vector3.TransformVector(_right, new Matrix4(rollMatrix));
        _up = Vector3.TransformVector(_up, new Matrix4(rollMatrix));
    }


    public void Update(KeyboardState input, MouseState mouse, float deltaTime)
    {
        if (input.IsKeyDown(Keys.W)) Transform.LocalPosition += Front * Speed * deltaTime;
        if (input.IsKeyDown(Keys.S)) Transform.LocalPosition -= Front * Speed * deltaTime;

        if (input.IsKeyDown(Keys.A)) Transform.LocalPosition -= Vector3.Normalize(Vector3.Cross(Front, Up)) * Speed * deltaTime;
        if (input.IsKeyDown(Keys.D)) Transform.LocalPosition += Vector3.Normalize(Vector3.Cross(Front, Up)) * Speed * deltaTime;

        if (input.IsKeyDown(Keys.Space)) Transform.LocalPosition += Up * Speed * deltaTime;
        if (input.IsKeyDown(Keys.LeftShift)) Transform.LocalPosition -= Up * Speed * deltaTime;

        if (_firstMove)
        {
            _lastPos = new Vector2(mouse.X, mouse.Y);
            _firstMove = false;
            return;
        }

        var deltaX = mouse.X - _lastPos.X;
        var deltaY = _lastPos.Y - mouse.Y;
        _lastPos = new Vector2(mouse.X, mouse.Y);

        Transform.LocalRotation.Y += deltaX * Sensitivity;
        Transform.LocalRotation.X += deltaY * Sensitivity;

        Transform.LocalRotation.X = MathHelper.Clamp(Transform.LocalRotation.X, -89f, 89f);

        UpdateVectors();
    }
}
