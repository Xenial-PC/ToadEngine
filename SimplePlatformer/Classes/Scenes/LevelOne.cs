using SimplePlatformer.Classes.GameObjects.Controllers;
using SimplePlatformer.Classes.GameObjects.Event;
using SimplePlatformer.Classes.GameObjects.Menus;
using SimplePlatformer.Classes.GameObjects.Scripts;
using SimplePlatformer.Classes.GameObjects.Scripts.World;
using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Objects.Skybox;
using ToadEngine.Classes.Base.Objects.View;
using static ToadEngine.Classes.Base.Rendering.RenderObject;

namespace SimplePlatformer.Classes.Scenes;

public class LevelOne() : Scene("Level1")
{
    private Skybox _skybox = null!;
    private Camera _camera = null!;
    private DirectionLight _directionLight = null!;
    private FPController _player = null!;

    private LevelGenerator _generator = null!;
    private Lava _outOfBoundsLava = null!;
    private List<GameObject> _level = null!;

    private PauseMenu _pauseMenu = null!;
    private EOLMenu _endOfLevelMenu = null!;

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
        AddService(_camera);

        _pauseMenu = new PauseMenu();
        Scripts.AddComponent(_pauseMenu);

        _endOfLevelMenu = new EOLMenu();
        Scripts.AddComponent(_endOfLevelMenu);

        _player = new FPController(new Vector3(0.3f, 2f, 0.3f))
        {
            Controller =
            {
                JumpHeight = 8f
            }
        };
        _player.GameObject.Transform.Position = new Vector3(0f, 14f, 0f);

        _directionLight = new DirectionLight();
        _directionLight.Settings.Direction = new Vector3(0f, -1f, 0);
        _directionLight.Transform.Rotation = new Vector3(-1f, -1.5f, -1f);

        _directionLight.Settings.Specular = new Vector3(0.3f);
        _directionLight.Settings.Ambient = new Vector3(0.5f);
        _directionLight.Settings.Diffuse = new Vector3(0.3f);

        _directionLight.AddShadowCaster();

        _generator = new LevelGenerator
        {
            OutOfBoundsRespawnScript = new RespawnScript
            {
                RespawnPosition = _player.GameObject.Transform.Position,
                Player = _player
            }
        };

        _outOfBoundsLava = new Lava(new Vector3(10000f, 1f, 10000f), new Vector3(0, -10f, 0),
            _generator.OutOfBoundsRespawnScript);
        
        _outOfBoundsLava.TGameObject.GameObject.Transform.LocalPosition.Y = 3;

        _level = _generator.GenerateLevelOne(_player);
    }

    public override void OnStart()
    {
        Instantiate(_skybox, InstantiateType.Late);
        Instantiate(_directionLight);

        Instantiate(_player.GameObject);
        Instantiate(_level);
        Instantiate(_outOfBoundsLava.GameObjects());
    }

    public override void OnUpdate(FrameEventArgs e)
    {
        var res = _outOfBoundsLava.Behavior as RespawnScript;
        res!.RespawnPosition = SavePointScript.SavePoint;

        if (PauseMenu.IsPaused) return;
        _camera.Update(WHandler.KeyboardState, WHandler.MouseState, (float)e.Time);
    }
}
