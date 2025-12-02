using SimplePlatformer.Classes.GameObjects.Controllers;
using SimplePlatformer.Classes.GameObjects.Menus;
using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Objects.Skybox;
using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Base.Scripting.Base;
using static ToadEngine.Classes.Base.Rendering.Object.RenderObject;

namespace SimplePlatformer.Classes.Scenes;

public class LevelTwo : Scene
{
    private Skybox _skybox = null!;
    private Camera _camera = null!;

    private SpotLight _flashLight;
    private FPController _player = null!;

    public PauseMenu PauseMenu = null!;
    public EOLMenu EndOfLevelMenu = null!;

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

        _camera = new Camera(WHandler.Size.X / (float)WHandler.Size.Y);
        Service.Add(_camera);

        _flashLight = new SpotLight();
        _player = new FPController(new Vector3(0.3f, 3f, 0.3f));

        PauseMenu = new PauseMenu();
        EndOfLevelMenu = new EOLMenu();

        Scripts.AddComponent(PauseMenu);
        Scripts.AddComponent(EndOfLevelMenu);
    }

    public override void OnStart()
    {
        Instantiate(_skybox, InstantiateType.Late);
        Instantiate(_flashLight);

        Instantiate(_player.GameObject);
    }

    public override void OnUpdate(FrameEventArgs e)
    {
        _flashLight.Settings.Direction = _camera.Front;
        _flashLight.Settings.Position = _player.GameObject.Transform.Position;
        
        if (PauseMenu.IsPaused) return;
        _camera.Update(WHandler.KeyboardState, WHandler.MouseState, (float)e.Time);
    }
}
