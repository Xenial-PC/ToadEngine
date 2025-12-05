using System.Numerics;
using System.Runtime.InteropServices;
using ToadEngine.Classes.Shaders;
using ToadEngine.Classes.Textures;
using ToadEngine.Classes.Textures.Base;
using static ToadEngine.Classes.Base.Assets.Mesh.MeshStructs;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace ToadEngine.Classes.Base.Assets;

public class Mesh
{
    public List<MeshStructs.Vertex> Vertices;
    public List<int> Indices;
    public Material? Material;
    public InternalMat InternalMat;

    private uint _vao, _vbo, _ebo;

    public Mesh(List<Vertex> vertices, List<int> indices, InternalMat mat)
    {
        Vertices = vertices;
        Indices = indices;
        InternalMat = mat;

        SetupMesh();
    }

    public void Draw(Shader shader)
    {
        shader.SetInt1("material.diffuse", 0);
        shader.SetInt1("material.specular", 1);
        shader.SetInt1("material.normals", 2);
        shader.SetInt1("material.height", 3);

        Material?.Diffuse?.Use(TextureUnit.Texture0);
        Material?.Specular?.Use(TextureUnit.Texture1);
        Material?.Normal?.Use(TextureUnit.Texture2);
        Material?.Height?.Use(TextureUnit.Texture3);

        if (Material == null) BaseTextures.White.Use();
        shader.SetInt1("material.hasNormalMap", Material?.Normal != null ? 1 : 0);

        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, Indices.Count, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
        GL.ActiveTexture(TextureUnit.Texture0);
    }

    private void SetupMesh()
    {
        GL.GenVertexArrays(1, out _vao);
        GL.GenBuffers(1, out _vbo);
        GL.GenBuffers(1, out _ebo);

        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer,
            Vertices.Count * Marshal.SizeOf<Vertex>(),
            Vertices.ToArray(),
            BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer,
            Indices.Count * sizeof(uint),
            Indices.ToArray(),
            BufferUsageHint.StaticDraw);

        var stride = Marshal.SizeOf<Vertex>();

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, IntPtr.Zero);

        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride,
            Marshal.OffsetOf<Vertex>("Normal"));

        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride,
            Marshal.OffsetOf<Vertex>("TexCoords"));

        GL.EnableVertexAttribArray(3);
        GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, stride,
            Marshal.OffsetOf<Vertex>("Tangent"));

        GL.EnableVertexAttribArray(4);
        GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, stride,
            Marshal.OffsetOf<Vertex>("Bitangent"));

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
            public Vector3 Tangent;
            public Vector3 Bitangent;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct InternalMat
        {
            public Vector3 Diffuse;
            public Vector3 Specular;
            public Vector3 Ambient;
            public Vector3 Normal;
            public float Shininess;
        }
    }
}
