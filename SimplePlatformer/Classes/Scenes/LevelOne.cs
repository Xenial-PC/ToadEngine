using SimplePlatformer.Classes.GameObjects.Controllers;
using SimplePlatformer.Classes.GameObjects.Materials;
using SimplePlatformer.Classes.GameObjects.Menus;
using SimplePlatformer.Classes.GameObjects.Models;
using SimplePlatformer.Classes.GameObjects.Scripts;
using SimplePlatformer.Classes.GameObjects.Scripts.World;
using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Objects.BuiltIn;
using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Objects.World;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Base.Scripting.Base;
using SavePointScript = SimplePlatformer.Classes.GameObjects.Scripts.SavePointScript;

namespace SimplePlatformer.Classes.Scenes;

public class LevelOne : Scene
{
    private Skybox _skybox = null!;
    private Camera _camera = null!;
    private DirectionLight _directionLight = null!;
    private Player _player = null!;

    private LevelGenerator _generator = null!;
    private readonly List<GameObject> _outOfBoundsLava = new();
    private List<GameObject> _level = null!;
    private GameObject _volcano = null!, _volcano2 = null!;

    public PauseMenu PauseMenu = null!;
    public EOLMenu EndOfLevelMenu = null!;

    public override void Setup()
    {
        SceneMaterials.LoadMaterials();

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

        PauseMenu = Scripts.AddComponent<PauseMenu>();
        EndOfLevelMenu = Scripts.AddComponent<EOLMenu>();

        _player = new Player();
        _player.Transform.Position = new Vector3(0f, 14f, 0f);
        _player.Name = "player";
        _player.AddComponent<FPController>();

        _directionLight = BuiltIns.Lights.DirectionLight();
        _directionLight.Settings.Direction = new Vector3(0f, -1f, 0);
        _directionLight.Transform.Rotation = new Vector3(-1f, -1.5f, -1f);

        _directionLight.Settings.Specular = new Vector3(0.3f);
        _directionLight.Settings.Ambient = new Vector3(0.5f);
        _directionLight.Settings.Diffuse = new Vector3(0.3f);

        RespawnScript.RespawnPosition = _player.Transform.Position;

        _generator = new LevelGenerator { Player = _player };
        _level = _generator.GenerateLevelOne(_player);

        GenerateLevelFloor();

        _volcano = new GameObject();
        _volcano.AddComponent<Volcano>();
        _volcano.Transform.Position = new Vector3(-1500, -20, 400);
        _volcano.Transform.LocalScale = new Vector3(30, 30, 30);

        _volcano2 = new GameObject();
        _volcano2.AddComponent<Volcano>();
        _volcano2.Transform.Position = new Vector3(1500, -20, 800);
        _volcano2.Transform.LocalScale = new Vector3(30, 30, 30);
    }

    private void GenerateLevelFloor()
    {
        for (var i = -5; i < 5; i++)
        {
            for (var j = -5; j < 5; j++) 
            {
                var lava = BuiltIns.Primitives.Cube();
                lava.AddComponent<TexturedCube>().Material = AssetManager.GetMaterial("LavaMat");
                
                lava.Transform.LocalScale = new Vector3(1000f, 1f, 1000f);
                lava.Transform.Position = new Vector3(i * 800, -10f, j * 800);

                var lavaScript = lava.AddComponent<LavaScript>();
                lavaScript.RespawnScript = lava.AddComponent<RespawnScript>();
                lavaScript.RespawnScript.Player = _player;
                lavaScript.RespawnScript.Controller = _player.GetComponent<FPController>()!;

                _outOfBoundsLava.Add(lava);
            }
        }
    }

    public override void OnStart()
    {
        Instantiate(_skybox.GameObject, InstantiateType.Late);
        Instantiate(_directionLight.GameObject);

        Instantiate(_player);
        Instantiate(_level);

        Instantiate(_volcano);
        Instantiate(_volcano2);

        foreach (var lava in _outOfBoundsLava) Instantiate(lava);
    }

    public override void OnUpdate(FrameEventArgs e)
    {
        RespawnScript.RespawnPosition = SavePointScript.SavePoint;

        if (PauseMenu.IsPaused) return;
        _camera.Update();
    }
}
