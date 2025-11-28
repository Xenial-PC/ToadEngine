namespace ToadEngine.Classes.Shaders;

public class Shader : IDisposable
{
    private bool _isDisposing;
    public int Handle;
    private readonly string _vert, _frag;

    public Shader(string vertex, string fragment)
    {
        _vert = vertex;
        _frag = fragment;

        var vertexShaderSource = RReader.ReadText(vertex);
        var fragmentShaderSource = RReader.ReadText(fragment);

        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);

        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);

        GL.CompileShader(vertexShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out var success);
        if (success == 0)
        {
            var infoLog = GL.GetShaderInfoLog(vertexShader);
            Console.WriteLine($"Error compiling shader:\n{infoLog}");
        }

        GL.CompileShader(fragmentShader);
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out success);
        if (success == 0)
        {
            var infoLog = GL.GetShaderInfoLog(fragmentShader);
            Console.WriteLine($"Error compiling shader:\n{infoLog}");
        }

        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);

        GL.LinkProgram(Handle);
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out success);
        if (success == 0)
        {
            var infoLog = GL.GetProgramInfoLog(Handle);
            Console.WriteLine($"Error Linking Program:\n{infoLog}");
        }

        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);

        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }

    public int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(Handle, attribName);
    }

    public void SetInt1(string name, int value)
    {
        var location = GL.GetUniformLocation(Handle, name);
        GL.Uniform1(location, value);
    }

    public void SetFloat1(string name, float value)
    {
        var location = GL.GetUniformLocation(Handle, name);
        GL.Uniform1(location, value);
    }

    public void SetDouble1(string name, double value)
    {
        var location = GL.GetUniformLocation(Handle, name);
        GL.Uniform1(location, value);
    }

    public void SetMatrix4(string name, Matrix4 value)
    {
        var location = GL.GetUniformLocation(Handle, name);
        GL.UniformMatrix4(location, true, ref value);
    }

    public void SetVector2(string name, Vector2 value)
    {
        var location = GL.GetUniformLocation(Handle, name);
        GL.Uniform2(location, value);
    }

    public void SetVector3(string name, Vector3 value)
    {
        var location = GL.GetUniformLocation(Handle, name);
        GL.Uniform3(location, value);
    }

    public void SetVector4(string name, Vector4 value)
    {
        var location = GL.GetUniformLocation(Handle, name);
        GL.Uniform4(location, value);
    }

    #region Reset

    protected virtual void Dispose(bool isDisposing)
    {
        if (_isDisposing) return;
        GL.DeleteProgram(Handle);
        _isDisposing = isDisposing;
    }

    ~Shader()
    {
        if (_isDisposing) return;
        Console.WriteLine($"GPU Resource Leak! Remember to call Dispose():\n{_vert}\n{_frag}");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
