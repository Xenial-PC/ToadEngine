using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Base.Scripting.Renderer;
using ToadEngine.Classes.Shaders;
using ToadEngine.Classes.Textures;

namespace ToadEngine.Classes.Base.Objects.World;

public class Skybox : MonoBehavior
{
    public SkyboxMaterial Material;

    private CubeMap? _skyboxCubeMap;
    private Shader? _skyboxShader;

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

    public void Awake()
    {
        _skyboxShader = ShaderManager.Add("Skybox", $"skybox.vert", $"skybox.frag");

        GL.GenVertexArrays(1, out _skyboxVao);
        GL.GenBuffers(1, out _skyboxVbo);
        GL.BindVertexArray(_skyboxVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _skyboxVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _skyboxVertices.Length * sizeof(float), _skyboxVertices, BufferUsageHint.StaticDraw);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), IntPtr.Zero);
        GL.BindVertexArray(0);

        _skyboxCubeMap = new CubeMap([
            Material.Right,
            Material.Left,
            Material.Top,
            Material.Bottom,
            Material.Front,
            Material.Back
        ]);

        _skyboxShader.Use();
        _skyboxShader.SetInt1("skybox", 0);

        var skyboxRenderer = AddComponent<SkyboxRenderer>();
        skyboxRenderer.SkyboxShader = _skyboxShader;
        skyboxRenderer.SkyboxVAO = _skyboxVao;
    }
}
