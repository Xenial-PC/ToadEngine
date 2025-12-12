using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Objects.Primitives;
using ToadEngine.Classes.Base.Objects.Skybox;
using ToadEngine.Classes.Base.Objects.View;
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

            _skybox = new Skybox
            ([
                $"{baseDirectory}Textures/level_one_skybox/right.png",
                $"{baseDirectory}Textures/level_one_skybox/left.png",
                $"{baseDirectory}Textures/level_one_skybox/top.png",
                $"{baseDirectory}Textures/level_one_skybox/bottom.png",
                $"{baseDirectory}Textures/level_one_skybox/front.png",
                $"{baseDirectory}Textures/level_one_skybox/back.png",
            ]);

            _camera = new Camera();
            Service.MainCamera = _camera;

            _directionLight = new DirectionLight();
            _directionLight.Settings.Direction = new Vector3(0f, -1f, 0);
            _directionLight.Transform.Rotation = new Vector3(-1f, -1.5f, -1f);

            _directionLight.Settings.Specular = new Vector3(0.3f);
            _directionLight.Settings.Ambient = new Vector3(0.5f);
            _directionLight.Settings.Diffuse = new Vector3(0.3f);

            var cube = new Cube();
            cube.Transform.Position = new Vector3(1f);
            cube.Transform.LocalScale = new Vector3(1f);
            Instantiate(cube);
        }

        public override void OnStart()
        {
            Instantiate(_skybox, InstantiateType.Late);
            Instantiate(_directionLight);
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
