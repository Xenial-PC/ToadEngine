using SimplePlatformer.Classes.GameObjects.Controllers;
using SimplePlatformer.Classes.GameObjects.Event;
using SimplePlatformer.Classes.GameObjects.Materials;
using SimplePlatformer.Classes.GameObjects.Menus;
using SimplePlatformer.Classes.GameObjects.Models;
using SimplePlatformer.Classes.GameObjects.Scripts;
using SimplePlatformer.Classes.GameObjects.Scripts.World;
using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Objects.Skybox;
using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.Scenes;

public class LevelOne : Scene
{
    private Skybox _skybox = null!;
    private Camera _camera = null!;
    private DirectionLight _directionLight = null!;
    private FPController _player = null!;

    private LevelGenerator _generator = null!;
    private List<Lava> _outOfBoundsLava = new();
    private List<GameObject> _level = null!;
    private Volcano _volcano = null!, _volcano2 = null!;

    public PauseMenu PauseMenu = null!;
    public EOLMenu EndOfLevelMenu = null!;

    public override void Setup()
    {
        SceneMaterials.SceneOne();

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

        PauseMenu = new PauseMenu();
        Scripts.AddComponent(PauseMenu);

        EndOfLevelMenu = new EOLMenu();
        Scripts.AddComponent(EndOfLevelMenu);

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

        _generator = new LevelGenerator
        {
            OutOfBoundsRespawnScript = new RespawnScript
            {
                Player = _player
            }
        };

        RespawnScript.RespawnPosition =  _player.GameObject.Transform.Position;

        for (var i = -5; i < 5; i++)
        {
            for (var j = -5; j < 5; j++)
            {
                var lava = new Lava(new Vector3(1000f, 1f, 1000f), new Vector3(i * 800, -10f, j * 800),
                    _generator.OutOfBoundsRespawnScript);

                lava.TGameObject.GameObject.Transform.LocalPosition.Y += 3;
                _outOfBoundsLava.Add(lava);
            }
        }
        
        _level = _generator.GenerateLevelOne(_player);

        _volcano = new Volcano();
        _volcano.Transform.Position = new Vector3(-1500, -20, 400);
        _volcano.Transform.LocalScale = new Vector3(30, 30, 30);

        _volcano2 = new Volcano();
        _volcano2.Transform.Position = new Vector3(1500, -20, 800);
        _volcano2.Transform.LocalScale = new Vector3(30, 30, 30);
    }

    public override void OnStart()
    {
        Instantiate(_skybox, InstantiateType.Late);
        Instantiate(_directionLight);

        Instantiate(_player.GameObject);
        Instantiate(_level);

        Instantiate(_volcano);
        Instantiate(_volcano2);

        foreach (var lava in _outOfBoundsLava) Instantiate(lava.GameObjects());
    }

    public override void OnUpdate(FrameEventArgs e)
    {
        RespawnScript.RespawnPosition = SavePointScript.SavePoint;

        if (PauseMenu.IsPaused) return;
        _camera.Update(WHandler.KeyboardState, WHandler.MouseState, (float)e.Time);
    }
}
