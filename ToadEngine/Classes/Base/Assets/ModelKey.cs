namespace ToadEngine.Classes.Base.Assets;

public readonly struct ModelKey : IEquatable<ModelKey>
{
    public readonly string Name;
    public readonly int MaterialHash;

    public ModelKey(string name, List<Material>? materials)
    {
        Name = name;

        if (materials == null)
        {
            MaterialHash = -1;
            return;
        }

        MaterialHash = materials.Aggregate(17, (acc, next) => acc * 31 + next.GetHashCode());
    }

    public bool Equals(ModelKey other) =>
        Name == other.Name && MaterialHash == other.MaterialHash;

    public override int GetHashCode() =>
        HashCode.Combine(Name, MaterialHash);
}
