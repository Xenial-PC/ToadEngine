using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Shaders;
using ToadEngine.Classes.Textures;

namespace ToadEngine.Classes.Base.Objects.Skybox;

/// <summary>
/// Takes In Order:
/// Right,
/// Left,
/// Top,
/// Bottom,
/// Front,
/// Back
/// </summary>
/// <param name="textures"></param>
public class Skybox(List<string> textures) : GameObject
{
    private CubeMap? _skyBox;
    private Shader? _skybox;

    private int _skyboxVao, _skyboxVbo;
    private readonly float[] _skyboxVertices =
    [
        -1.0f,  1.0f, -1.0f,
        -1.0f, -1.0f, -1.0f,
        1.0f, -1.0f, -1.0f,
        1.0f, -1.0f, -1.0f,
        1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,
        -1.0f, -1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f,  1.0f,
        -1.0f, -1.0f,  1.0f,

        1.0f, -1.0f, -1.0f,
        1.0f, -1.0f,  1.0f,
        1.0f,  1.0f,  1.0f,
        1.0f,  1.0f,  1.0f,
        1.0f,  1.0f, -1.0f,
        1.0f, -1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,
        -1.0f,  1.0f,  1.0f,
        1.0f,  1.0f,  1.0f,
        1.0f,  1.0f,  1.0f,
        1.0f, -1.0f,  1.0f,
        -1.0f, -1.0f,  1.0f,

        -1.0f,  1.0f, -1.0f,
        1.0f,  1.0f, -1.0f,
        1.0f,  1.0f,  1.0f,
        1.0f,  1.0f,  1.0f,
        -1.0f,  1.0f,  1.0f,
        -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f, -1.0f,
        -1.0f, -1.0f,  1.0f,
        1.0f, -1.0f, -1.0f,
        1.0f, -1.0f, -1.0f,
        -1.0f, -1.0f,  1.0f,
        1.0f, -1.0f,  1.0f
    ];

    public override void Setup()
    {
        _skybox = new Shader($"skybox.vert", $"skybox.frag");

        GL.GenVertexArrays(1, out _skyboxVao);
        GL.GenBuffers(1, out _skyboxVbo);
        GL.BindVertexArray(_skyboxVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _skyboxVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _skyboxVertices.Length * sizeof(float), _skyboxVertices, BufferUsageHint.StaticDraw);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), IntPtr.Zero);
        GL.BindVertexArray(0);

        _skyBox = new CubeMap(textures);

        _skybox.Use();
        _skybox.SetInt1("skybox", 0);
    }

    public override void Draw(float deltaTime)
    {
        var camera = Service.MainCamera;

        GL.DepthFunc(DepthFunction.Lequal);
        GL.DepthMask(false);

        _skybox!.Use();

        _skybox.SetMatrix4("view", new Matrix4(new Matrix3(camera.GetViewMatrix())));
        _skybox.SetMatrix4("projection", camera.GetProjectionMatrix());

        GL.BindVertexArray(_skyboxVao);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.TextureCubeMap, _skyBox!.Handle);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        GL.BindVertexArray(0);

        GL.DepthMask(true);
        GL.DepthFunc(DepthFunction.Less);
    }

    public override void Update(float deltaTime)
    {
    }

    public override void Dispose()
    {
        _skybox?.Dispose();
    }
}
