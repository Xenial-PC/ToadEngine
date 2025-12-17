using SimplePlatformer.Classes.GameObjects.Controllers;
using SimplePlatformer.Classes.GameObjects.Event;
using SimplePlatformer.Classes.GameObjects.Models;
using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Objects.BuiltIn;
using ToadEngine.Classes.Base.Rendering.Object;

namespace SimplePlatformer.Classes.GameObjects.Scripts.World;

public class LevelGenerator
{
    public Player Player;

    private Vector3 _lastPosition;
    private bool _isFirstPlatform = true;
    private bool _flipDirection, _lastWasLava;

    #region Level One

    public List<GameObject> GenerateLevelOne(Player player)
    {
        var level = new List<GameObject>();

        var savePoint = BuiltIns.Primitives.Cube();
        savePoint.AddComponent<TexturedCube>().Material = AssetManager.GetMaterial("ConcreteMat");
        var savePointScript = savePoint.AddComponent<SavePointScript>();

        savePoint.Transform.Position = new Vector3(0f);
        savePoint.Transform.LocalScale = new Vector3(4f, 12f, 4f);

        level.AddRange(savePoint);
        _lastPosition = new Vector3(0f);

        var rand = new Random();
        var parts = rand.Next(15, 20);
        for (var i = 0; i < parts; i++)
        {
            level.AddRange(GeneratePart(i == parts - 1));
        }

        return level;
    }

    private List<GameObject> GeneratePart(bool isLastSavePoint)
    {
        var rand = new Random();
        var parts = new List<GameObject>();
        for (var i = 0; i < rand.Next(4, 15); i++)
        {
            var placement = rand.Next(-3, 3);
            if ((placement < 0 && !_lastWasLava) || _isFirstPlatform)
            {
                parts.AddRange(CreatePlatform(rand));
                continue;
            }

            var lavaChance = rand.Next(-3, 15);
            parts.Add(lavaChance > 0 || _lastWasLava ? CreateMovingPlatform(rand) : CreateLava(rand));
        }

        parts.AddRange(CreateSavePoint(rand, isLastSavePoint));
        return parts;
    }

    private GameObject CreatePlatform(Random rand)
    {
        var platform = BuiltIns.Primitives.Cube();
        platform.AddComponent<TexturedCube>().Material = AssetManager.GetMaterial("GraniteMat");
        platform.Transform.LocalScale = new Vector3(3f, 14f, 3f);

        var platformScript = platform.AddComponent<PlatformScript>();

        var maxHeight = Math.Max(_lastPosition.Y + rand.Next(1, 4), 8f);
        if (maxHeight >= 8f) maxHeight = rand.Next(-3, 0);

        var distance = Math.Max(_lastPosition.Z + rand.Next(5, 13), _lastPosition.Z + 10f);

        var pos = new Vector3(_isFirstPlatform ? 0 : _flipDirection ? rand.Next(2, 4) : rand.Next(-4, -2) , maxHeight, _isFirstPlatform ? 5f : distance);
        platform.Transform.Position = pos;

        _lastPosition = platform.Transform.Position;

        _isFirstPlatform = false;
        _flipDirection = !_flipDirection;

        return platform;
    }

    private GameObject CreateMovingPlatform(Random rand)
    {
        var platform = BuiltIns.Primitives.Cube();
        platform.AddComponent<TexturedCube>().Material = AssetManager.GetMaterial("GraniteMat");
        platform.Transform.LocalScale = new Vector3(3f, 14f, 3f);

        var platformScript = platform.AddComponent<PlatformScript>();
        var mPlatform = platform.AddComponent<MovingPlatform>();
        mPlatform.MovingRange = new Vector3(_lastWasLava ? 0 : rand.Next(-6, 6), 0f, _lastWasLava ? -11.3f : 0);
        mPlatform.MovingSpeed = 0.4f;
        mPlatform.Player = Player;
        mPlatform.Controller = Player.GetComponent<FPController>()!;

        var maxHeight = Math.Max(_lastPosition.Y + rand.Next(1, 4), 8f);
        if (maxHeight >= 8f) maxHeight = rand.Next(-3, 0);

        var distance = Math.Max(_lastPosition.Z + rand.Next(5, 13), _lastPosition.Z + 10f);
        
        var pos = new Vector3(_isFirstPlatform ? 0 : _flipDirection ? rand.Next(2, 4) : rand.Next(-4, -2), maxHeight, _isFirstPlatform ? 5f : distance);
        platform.Transform.Position = pos;
        
        _lastPosition = platform.Transform.Position;
        
        _isFirstPlatform = false;
        _flipDirection = !_flipDirection;

        _lastWasLava = false;

        return platform;
    }

    private GameObject CreateLava(Random rand)
    {
        _lastWasLava = true;

        var maxHeight = Math.Max(_lastPosition.Y + rand.Next(1, 4), 8f);
        if (maxHeight >= 8f) maxHeight = rand.Next(-3, 0);

        var distance = Math.Max(_lastPosition.Z + rand.Next(5, 13), _lastPosition.Z + 10f);
        var pos = new Vector3(_flipDirection ? rand.Next(2, 4) : rand.Next(-4, -2), maxHeight, _isFirstPlatform ? 5f : distance);

        var lava = BuiltIns.Primitives.Cube();
        lava.AddComponent<TexturedCube>().Material = AssetManager.GetMaterial("LavaMat");
        lava.Transform.Position = pos;
        lava.Transform.LocalScale = new Vector3(5f, 14f, 5f);

        var lavaScript = lava.AddComponent<LavaScript>();
        lavaScript.RespawnScript = lava.AddComponent<RespawnScript>();
        lavaScript.RespawnScript.Player = Player;
        lavaScript.RespawnScript.Controller = Player.GetComponent<FPController>()!;

        _lastPosition = lava.Transform.Position;

        _flipDirection = !_flipDirection;
        return lava;
    }

    private GameObject CreateSavePoint(Random rand, bool isLastSavePoint)
    {
        var maxHeight = Math.Max(_lastPosition.Y + rand.Next(1, 4), 8f);
        if (maxHeight >= 8f) maxHeight = rand.Next(-3, 0);

        var distance = Math.Max(_lastPosition.Z + rand.Next(5, 13), _lastPosition.Z + 10f);
        var pos = new Vector3(_flipDirection ? rand.Next(2, 4) : rand.Next(-4, -2), maxHeight, _isFirstPlatform ? 5f : distance);

        var savePoint = BuiltIns.Primitives.Cube();
        savePoint.AddComponent<TexturedCube>().Material = AssetManager.GetMaterial("ConcreteMat");
        var savePointScript = savePoint.AddComponent<SavePointScript>();

        savePoint.Transform.Position = pos;
        savePoint.Transform.LocalScale = new Vector3(6f, 12f, 6f);
        savePointScript.IsLastSavePoint = isLastSavePoint;

        _lastPosition = savePoint.Transform.Position;
        _flipDirection = !_flipDirection;

        return savePoint;
    }

    #endregion
}
