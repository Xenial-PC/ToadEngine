using SimplePlatformer.Classes.GameObjects.Controllers;
using SimplePlatformer.Classes.GameObjects.Menus;
using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Objects.Skybox;
using ToadEngine.Classes.Base.Objects.View;
using static ToadEngine.Classes.Base.Rendering.RenderObject;

namespace SimplePlatformer.Classes.Scenes;

public class LevelTwo() : Scene("Level2")
{
    private Skybox _skybox = null!;
    private Camera _camera = null!;

    private SpotLight _flashLight;
    private FPController _player = null!;

    private PauseMenu _pauseMenu = null!;
    private EOLMenu _endOfLevelMenu = null!;

    public override void Setup()
    {
        base.Setup();
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
        AddService(_camera);

        _flashLight = new SpotLight();
        _player = new FPController(new Vector3(0.3f, 3f, 0.3f));

        _pauseMenu = new PauseMenu();
        _endOfLevelMenu = new EOLMenu();

        Scripts.AddComponent(_pauseMenu);
        Scripts.AddComponent(_endOfLevelMenu);
    }

    public override void OnStart()
    {
        base.OnStart();
        Instantiate(_skybox, InstantiateType.Late);
        Instantiate(_flashLight);

        Instantiate(_player.GameObject);
    }

    public override void OnDraw(float deltaTime)
    {
        base.OnDraw(deltaTime);
    }

    public override void OnUpdate(FrameEventArgs e)
    {
        base.OnUpdate(e);
        _flashLight.Settings.Direction = _camera.Front;
        _flashLight.Settings.Position = _player.GameObject.Transform.Position;
        
        if (PauseMenu.IsPaused) return;
        _camera.Update(WHandler.KeyboardState, WHandler.MouseState, (float)e.Time);
    }

    public override void OnLateUpdate(FrameEventArgs e)
    {
        base.OnLateUpdate(e);
    }

    public override void OnResize(FramebufferResizeEventArgs e)
    {
        base.OnResize(e);
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}
