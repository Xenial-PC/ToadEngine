using ToadEngine.Classes.Base.Objects.BuiltIn;
using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Objects.World;
using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEditor.Classes.EditorCore.Scenes
{
    public class TestScene : Scene
    {
        private Skybox _skybox = null!;
        private Camera _camera = null!;
        private DirectionLight _directionLight = null!;

        public override void Setup()
        {
            var baseDirectory = $"{Directory.GetCurrentDirectory()}/Resources/";

            _skybox = BuiltIns.World.Skybox();
            _skybox.Material = new SkyboxMaterial()
            {
                Right = $"{baseDirectory}Textures/level_one_skybox/right.png",
                Left = $"{baseDirectory}Textures/level_one_skybox/left.png",
                Top = $"{baseDirectory}Textures/level_one_skybox/top.png",
                Bottom = $"{baseDirectory}Textures/level_one_skybox/bottom.png",
                Front = $"{baseDirectory}Textures/level_one_skybox/front.png",
                Back = $"{baseDirectory}Textures/level_one_skybox/back.png",
            };

            _camera = new Camera();
            Service.MainCamera = _camera;

            _directionLight = BuiltIns.Lights.DirectionLight();
            _directionLight.Settings.Direction = new Vector3(0f, -1f, 0);
            _directionLight.Transform.Rotation = new Vector3(-1f, -1.5f, -1f);

            _directionLight.Settings.Specular = new Vector3(0.3f);
            _directionLight.Settings.Ambient = new Vector3(0.5f);
            _directionLight.Settings.Diffuse = new Vector3(0.3f);

           

            for (var i = 0; i < 32; i++)
            {
                var cube = BuiltIns.Primitives.Cube();
                cube.Transform.Position = new Vector3(30 / 10f * i);
                Instantiate(cube);
            }
        }

        public override void OnStart()
        {
            var cube = BuiltIns.Primitives.Cube();
            cube.Name = "Parent Cube";
            cube.Transform.Position = new Vector3(1f);

            var childCube = BuiltIns.Primitives.Cube();
            childCube.Name = "Child Cube";
            childCube.Transform.LocalPosition.Y += 2f;

            var childCube2 = BuiltIns.Primitives.Cube();
            childCube2.Name = "Child Cube 2";
            childCube2.Transform.LocalPosition.Y += 2f;

            var childCube3 = BuiltIns.Primitives.Cube();
            childCube3.Name = "Child Cube 3";
            childCube3.Transform.LocalPosition.Y += 2f;

            cube.AddChild(childCube);
            cube.AddChild(childCube3);

            childCube.AddChild(childCube2);

            Instantiate([cube, childCube, childCube2, childCube3]);

            Instantiate(_skybox.GameObject, InstantiateType.Late);
            Instantiate(_directionLight.GameObject);
        }

        public override void OnUpdate(FrameEventArgs e)
        {
            _camera.Update();
        }

        public override void OnResize(FramebufferResizeEventArgs e)
        {
        }

        public override void Dispose()
        {
        }
    }
}
