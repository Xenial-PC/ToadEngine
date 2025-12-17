using SimplePlatformer.Classes.GameObjects.Controllers;
using SimplePlatformer.Classes.GameObjects.Menus;
using ToadEngine.Classes.Base.Objects.BuiltIn;
using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Objects.World;
using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.Scenes;

public class LevelTwo : Scene
{
    private Skybox _skybox = null!;
    private Camera _camera = null!;

    private SpotLight _flashLight;
    private Player _player = null!;

    public PauseMenu PauseMenu = null!;
    public EOLMenu EndOfLevelMenu = null!;

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
        Service.Add(_camera);

        _player = new Player();

        PauseMenu = new PauseMenu();
        EndOfLevelMenu = new EOLMenu();

        Scripts.AddComponent(PauseMenu);
        Scripts.AddComponent(EndOfLevelMenu);
    }

    public override void OnStart()
    {
        Instantiate(_skybox.GameObject, InstantiateType.Late);
        
        Instantiate(_player);
    }

    public override void OnUpdate(FrameEventArgs e)
    {
        _flashLight.Settings.Direction = _camera.Front;
        _flashLight.Settings.Position = _player.Transform.Position;
        
        if (PauseMenu.IsPaused) return;
        _camera.Update();
    }
}
