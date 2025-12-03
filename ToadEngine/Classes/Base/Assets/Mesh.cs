using System.Numerics;
using System.Runtime.InteropServices;
using ToadEngine.Classes.Shaders;
using ToadEngine.Classes.Textures;
using ToadEngine.Classes.Textures.Base;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace ToadEngine.Classes.Base.Assets;

public class Mesh
{
    public List<MeshStructs.Vertex> Vertices;
    public List<int> Indices;
    public List<Texture> Textures;
    public MeshStructs.Mat Material;

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
        uint normalNr = 1;
        uint heightNr = 1;
        
        for (var i = 0; i < Textures.Count; i++)
        {
            GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + i));
            var number = string.Empty;
            var name = Textures[i].TypeName;

            number = name switch
            {
                "texture_diffuse" => $"{diffuseNr++}",
                "texture_specular" => $"{specularNr++}",
                "texture_normal" => $"{normalNr++}",
                "texture_height" => $"{heightNr++}",
                _ => number
            };

            shader.SetInt1("material.".Replace("texture_", string.Empty) + name + number, i);
            GL.BindTexture(TextureTarget.Texture2D, Textures[i].Handle);
        }

        if (Textures.Count <= 0) BaseTextures.White.Use();
        
        shader.SetVector3("materials.ambient", Material.Ambient);
        shader.SetVector3("materials.diffuse", Material.Diffuse);
        shader.SetVector3("materials.specular", Material.Specular);
        shader.SetVector3("materials.normal", Material.Normal);
        shader.SetFloat1("materials.shininess", Material.Shininess);

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

        GL.EnableVertexAttribArray(3);
        GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, stride,
            Marshal.OffsetOf<MeshStructs.Vertex>("Tangent"));

        GL.EnableVertexAttribArray(4);
        GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, stride,
            Marshal.OffsetOf<MeshStructs.Vertex>("Bitangent"));

        GL.BindVertexArray(0);
    }

    public class MeshStructs
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Vertex
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector3 Tangent;
            public Vector3 Bitangent;
            public Vector2 TexCoords;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Mat
        {
            public Vector3 Diffuse;
            public Vector3 Specular;
            public Vector3 Ambient;
            public Vector3 Normal;
            public float Shininess;
        }
    }
}
