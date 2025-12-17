using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Objects.Primitives;
using ToadEngine.Classes.Base.Objects.World;
using ToadEngine.Classes.Base.Rendering.Object;

namespace ToadEngine.Classes.Base.Objects.BuiltIn;

public static class BuiltIns
{
    public static class Lights
    {
        public static DirectionLight DirectionLight()
        {
            var dirLightGO = new GameObject();
            return dirLightGO.AddComponent<DirectionLight>();
        }

        public static PointLight PointLight()
        {
            var pointLightGO = new GameObject();
            return pointLightGO.AddComponent<PointLight>();
        }

        public static SpotLight SpotLight()
        {
            var spotLightGO = new GameObject();
            return spotLightGO.AddComponent<SpotLight>();
        }
    }

    public static class Primitives
    {
        public static GameObject Cube()
        {
            var cubeGO = new GameObject();
            cubeGO.AddComponent<CubeMesh>();
            return cubeGO;
        }

        public static GameObject Cone()
        {
            var coneGO = new GameObject();
            coneGO.AddComponent<ConeMesh>();
            return coneGO;
        }

        public static GameObject Cylinder()
        {
            var cylinderGO = new GameObject();
            cylinderGO.AddComponent<CylinderMesh>();
            return cylinderGO;
        }

        public static GameObject Sphere()
        {
            var sphereGO = new GameObject();
            sphereGO.AddComponent<SphereMesh>();
            return sphereGO;
        }
    }

    public static class World
    {
        public static Skybox Skybox()
        {
            var skyboxGO = new GameObject();
            return skyboxGO.AddComponent<Skybox>();
        }
    }
}
