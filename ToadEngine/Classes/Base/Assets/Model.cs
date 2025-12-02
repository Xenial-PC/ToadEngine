using Assimp;
using ToadEngine.Classes.Shaders;
using ToadEngine.Classes.Textures;
using static ToadEngine.Classes.Base.Assets.Mesh;

namespace ToadEngine.Classes.Base.Assets;

public class Model
{
    private readonly List<Mesh> _meshes = new();
    private readonly string _directory, _path;

    public Model(string path, string model)
    {
        _directory = Path.Combine(path, model);
        _path = path;

        LoadModel();
    }

    public void Draw(Shader shader)
    {
        foreach (var mesh in _meshes)
            mesh.Draw(shader);
    }

    private void LoadModel()
    {
        var context = new AssimpContext();
        var scene = context.ImportFile(_directory, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.CalculateTangentSpace);
        if (scene == null || scene.SceneFlags == SceneFlags.Incomplete || scene.RootNode == null)
        {
            Console.WriteLine($"Error.Assimp.Loading.Model");
            return;
        }

        ProcessNode(scene.RootNode, scene);
    }

    private void ProcessNode(Node node, Assimp.Scene scene)
    {
        for (var i = 0; i < node.MeshCount; i++)
        {
            var mesh = scene.Meshes[node.MeshIndices[i]];
            _meshes.Add(ProcessMesh(mesh, scene));
        }

        foreach (var n in node.Children) ProcessNode(n, scene);
    }

    private Mesh ProcessMesh(Assimp.Mesh mesh, Assimp.Scene scene)
    {
        List<MeshStructs.Vertex> vertices = new();
        List<int> indices = new();
        List<Texture> textures = new();
        MeshStructs.Mat matData = new();

        for (var i = 0; i < mesh.Vertices.Count; i++)
        {
            MeshStructs.Vertex vertex = new();
            var vector = new Vector3()
            {
                X = mesh.Vertices[i].X,
                Y = mesh.Vertices[i].Y,
                Z = mesh.Vertices[i].Z
            };
            vertex.Position = vector;

            if (mesh.HasNormals)
            {
                vector.X = mesh.Normals[i].X;
                vector.Y = mesh.Normals[i].Y;
                vector.Z = mesh.Normals[i].Z;
                vertex.Normal = vector;
            }

            if (mesh.HasTextureCoords(0))
            {
                var texCoords = new Vector2()
                {
                    X = mesh.TextureCoordinateChannels[0][i].X,
                    Y = mesh.TextureCoordinateChannels[0][i].Y,
                };
                vertex.TexCoords = texCoords;

                vector.X = mesh.Tangents[i].X;
                vector.Y = mesh.Tangents[i].Y;
                vector.Z = mesh.Tangents[i].Z;
                vertex.Tangent = vector;

                vector.X = mesh.BiTangents[i].X;
                vector.Y = mesh.BiTangents[i].Y;
                vector.Z = mesh.BiTangents[i].Z;
                vertex.Bitangent = vector;
            }
            else vertex.TexCoords = new Vector2(0.0f);
            vertices.Add(vertex);
        }
        
        foreach (var faceIndices in mesh.Faces) indices.AddRange(faceIndices.Indices);
        if (mesh.MaterialIndex < 0) return new Mesh(vertices, indices, textures, matData);

        var material = scene.Materials[mesh.MaterialIndex];
        matData = LoadMaterial(material);
        
        var diffuseMaps = LoadMaterialTextures(material, TextureType.Diffuse, "texture_diffuse");
        textures.AddRange(diffuseMaps);

        var specularMaps = LoadMaterialTextures(material, TextureType.Specular, "texture_specular");
        textures.AddRange(specularMaps);

        var normalMaps = LoadMaterialTextures(material, TextureType.Height, "texture_normal");
        textures.AddRange(normalMaps);

        var heightMaps = LoadMaterialTextures(material, TextureType.Ambient, "texture_height");
        textures.AddRange(heightMaps);

        return new Mesh(vertices, indices, textures, matData);
    }

    private List<Texture> LoadMaterialTextures(Material mat, TextureType type, string typeName)
    {
        List<Texture> textures = new();
        for (var i = 0; i < mat.GetMaterialTextureCount(type); i++)
        {
            mat.GetMaterialTexture(type, i, out var slot);
            var texture = new Texture($"{_path}/{slot.FilePath}", type == TextureType.Diffuse);
            texture.Handle = texture.Handle;
            texture.Type = typeName;
            texture.Path = slot.FilePath;
            textures.Add(texture);
        }

        return textures;
    }

    MeshStructs.Mat LoadMaterial(Material mat)
    {
        var material = new MeshStructs.Mat();
        if (mat.HasColorDiffuse)
            material.Diffuse = new Vector3(mat.ColorDiffuse.R, mat.ColorDiffuse.G, mat.ColorDiffuse.B);

        if (mat.HasColorAmbient)
            material.Ambient = new Vector3(mat.ColorAmbient.R, mat.ColorAmbient.G, mat.ColorAmbient.B);

        if (mat.HasColorSpecular)
            material.Specular = new Vector3(mat.ColorSpecular.R, mat.ColorSpecular.G, mat.ColorSpecular.B);

        if (mat.HasShininess)
            material.Shininess = mat.Shininess;

        return material;
    }
}
