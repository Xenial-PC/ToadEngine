using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Objects.Primitives;
using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Objects.World;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Objects.BuiltIn;

public static class BuiltIn
{
    /// <summary>
    /// Holds all base lights, Direction, Point, Spot
    /// </summary>
    public static class Lights
    {
        public static DirectionLight DirectionLight => new GameObject().AddComponent<DirectionLight>();

        public static PointLight PointLight => new GameObject().AddComponent<PointLight>();

        public static SpotLight SpotLight => new GameObject().AddComponent<SpotLight>();
    }

    /// <summary>
    /// Holds simple Primitives, Cube, Cone, Etc
    /// </summary>
    public static class Primitives
    {
        public static GameObject Cube => new GameObject().AddComponent<CubeMesh>().GameObject;

        public static GameObject Cone => new GameObject().AddComponent<ConeMesh>().GameObject;

        public static GameObject Cylinder => new GameObject().AddComponent<CylinderMesh>().GameObject;

        public static GameObject Sphere => new GameObject().AddComponent<SphereMesh>().GameObject;
    }

    /// <summary>
    /// Holds world objects -> Skybox, Camera, Etc
    /// </summary>
    public static class World
    {
        /// <summary>
        /// Creates a new empty skybox Object
        /// </summary>
        /// <returns></returns>
        public static Skybox Skybox => new GameObject().AddComponent<Skybox>();

        /// <summary>
        /// Creates a new camera with a defined name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Camera Camera(string name) => new GameObject { Name = name }.AddComponent<Camera>();

        /// <summary>
        /// Creates the default main camera
        /// </summary>
        /// <returns></returns>
        public static Camera CreateMainCamera()
        {
            var camera = new GameObject { Name = "Main Camera" };
            camera.AddTag("MainCamera");

            var cameraComponent = camera.AddComponent<Camera>();
            Service.Scene.Instantiate(camera);
            return cameraComponent;
        }
    }
}
