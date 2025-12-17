using ToadEngine.Classes.Base.Scripting.Base;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace ToadEngine.Classes.Base.Objects.Lights;

public class Light : MonoBehavior
{
    public struct DirectionLight
    {
        public Vector3 Direction;
        public Vector3 Ambient;
        public Vector3 Diffuse;
        public Vector3 Specular;
    }

    public struct PointLight
    {
        public Vector3 Position;

        public float Constant;
        public float Linear;
        public float Quadratic;

        public Vector3 Ambient;
        public Vector3 Diffuse;
        public Vector3 Specular;
    }

    public struct SpotLight
    {
        public Vector3 Position;
        public Vector3 Direction;

        public float CutOff;
        public float OuterCutOff;

        public Vector3 Ambient;
        public Vector3 Diffuse;
        public Vector3 Specular;

        public float Constant;
        public float Linear;
        public float Quadratic;
    }
}
