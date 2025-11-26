using System.Runtime.InteropServices;
using ToadEngine.Classes.Shaders;
using ToadEngine.Classes.Textures;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace ToadEngine.Classes.Base.Assets;

public class Mesh
{
    public List<MeshStructs.Vertex> Vertices = new();
    public List<int> Indices = new();
    public List<Texture> Textures = new();
    public MeshStructs.Mat Material = new();

    private uint _vao, _vbo, _ebo;

    public Mesh(List<MeshStructs.Vertex> vertices, List<int> indices, List<Texture> textures, MeshStructs.Mat material)
    {
        Vertices = vertices;
        Indices = indices;
        Textures = textures;
        Material = material;

        SetupMesh();
    }

    public void Draw(Shader shader)
    {
        uint diffuseNr = 1;
        uint specularNr = 1;
        for (var i = 0; i < Textures.Count; i++)
        {
            GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + i));
            var number = string.Empty;
            var name = Textures[i].Type;

            if (name == "texture_diffuse")
                number = $"{diffuseNr++}";
            else if (name == "texture_specular")
                number = $"{specularNr++}";

            shader.SetInt1("material.".Replace("texture_", string.Empty) + name + number, i); // 
            GL.BindTexture(TextureTarget.Texture2D, Textures[i].Handle);
        }
        //GL.ActiveTexture(TextureUnit.Texture0);

        shader.SetVector3("materials.ambient", Material.Ambient);
        shader.SetVector3("materials.diffuse", Material.Diffuse);
        shader.SetVector3("materials.specular", Material.Specular);
        shader.SetFloat1("materials.shininess", Material.Shininess);

        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, Indices.Count, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }

    private void SetupMesh()
    {
        GL.GenVertexArrays(1, out _vao);
        GL.GenBuffers(1, out _vbo);
        GL.GenBuffers(1, out _ebo);

        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer,
            Vertices.Count * Marshal.SizeOf<MeshStructs.Vertex>(),
            Vertices.ToArray(),
            BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer,
            Indices.Count * sizeof(uint),
            Indices.ToArray(),
            BufferUsageHint.StaticDraw);

        var stride = Marshal.SizeOf<MeshStructs.Vertex>();

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, IntPtr.Zero);

        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride,
            Marshal.OffsetOf<MeshStructs.Vertex>("Normal"));

        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride,
            Marshal.OffsetOf<MeshStructs.Vertex>("TexCoords"));

        GL.BindVertexArray(0);
    }


    public class MeshStructs
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Vertex
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 TexCoords;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Mat
        {
            public Vector3 Diffuse;
            public Vector3 Specular;
            public Vector3 Ambient;
            public float Shininess;
        }
    }
}
