using Assimp;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Shaders;
using ToadEngine.Classes.Textures;
using static ToadEngine.Classes.Base.Assets.Mesh;

namespace ToadEngine.Classes.Base.Assets;

public class Model
{
    public readonly List<Mesh> Meshes = new();
    private readonly string _directory, _path;

    public List<Material?> GetMaterials => Meshes.Select(m => m.Material).ToList();
    
    public Model(string path, string model)
    {
        _directory = Path.Combine(path, model);
        _path = path;
        
        LoadModel();
    }

    public Model(string model)
    {
        var modelStream = RReader.ReadAsMemoryStream(model);
        LoadModel(modelStream!, Path.GetExtension(model).TrimStart('.'));
    }

    public static Model Load(string path, string model)
    {
        var newModel = new Model(path, model);
        return newModel;
    }

    public static Model Load(string model)
    {
        var newModel = new Model(model);
        return newModel;
    }

    public void SetMaterial(Material material, int index) => GetMaterials[index] = material;
    public void SetMaterials(List<Material> materials) => UpdateMaterials(materials);

    private void UpdateMaterials(List<Material> materials)
    {
        for (var i = 0; i < materials.Count; i++)
        {
            if (i >= Meshes.Count) return;
            Meshes[i].Material = materials[i];
        }
    }

    public void Draw(Shader shader)
    {
        foreach (var mesh in Meshes)
            mesh.Draw(shader);
    }

    private void LoadModel(Stream stream, string format)
    {
        var context = new AssimpContext();
        var scene = context.ImportFileFromStream(stream, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.CalculateTangentSpace, format);
        if (scene == null || scene.SceneFlags == SceneFlags.Incomplete || scene.RootNode == null)
        {
            Console.WriteLine($"Error.Assimp.Loading.Model");
            return;
        }

        ProcessNode(scene.RootNode, scene);
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
            Meshes.Add(ProcessMesh(mesh, scene));
        }

        foreach (var n in node.Children) ProcessNode(n, scene);
    }

    private Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
    {
        List<MeshStructs.Vertex> vertices = new();
        List<int> indices = new();
        MeshStructs.InternalMat matData = new();

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

                if (mesh.HasTangentBasis)
                {
                    vector.X = mesh.Tangents[i].X;
                    vector.Y = mesh.Tangents[i].Y;
                    vector.Z = mesh.Tangents[i].Z;
                    vertex.Tangent = vector;

                    vector.X = mesh.BiTangents[i].X;
                    vector.Y = mesh.BiTangents[i].Y;
                    vector.Z = mesh.BiTangents[i].Z;
                    vertex.Bitangent = vector;
                }
            }
            else vertex.TexCoords = new Vector2(0.0f);
            vertices.Add(vertex);
        }
        
        foreach (var faceIndices in mesh.Faces) indices.AddRange(faceIndices.Indices);
        if (mesh.MaterialIndex < 0) return new Mesh(vertices, indices, matData);

        var material = scene.Materials[mesh.MaterialIndex];
        matData = LoadMaterial(material);

        return new Mesh(vertices, indices, matData);
    }

    MeshStructs.InternalMat LoadMaterial(Assimp.Material mat)
    {
        var material = new MeshStructs.InternalMat();
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
