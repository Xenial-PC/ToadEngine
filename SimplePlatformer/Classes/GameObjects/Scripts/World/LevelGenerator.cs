using NAudio.Codecs;
using SimplePlatformer.Classes.GameObjects.Controllers;
using SimplePlatformer.Classes.GameObjects.Event;
using System.Collections.Generic;
using ToadEngine.Classes.Base.Rendering.Object;
using Platform = SimplePlatformer.Classes.GameObjects.World.Platform;

namespace SimplePlatformer.Classes.GameObjects.Scripts.World;

public class LevelGenerator
{
    public RespawnScript OutOfBoundsRespawnScript = null!;

    private Vector3 _lastPosition;
    private bool _isFirstPlatform = true;
    private bool _flipDirection, _lastWasLava;

    #region Level One

    public List<GameObject> GenerateLevelOne(FPController player)
    {
        var level = new List<GameObject>();

        level.AddRange(new SavePoint(new Vector3(0, 0, 0), new Vector3(4f, 12f, 4f)).GameObjects());
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
            parts.AddRange(lavaChance > 0 || _lastWasLava ? CreateMovingPlatform(rand) : CreateLava(rand));
        }

        parts.AddRange(CreateSavePoint(rand, isLastSavePoint));
        return parts;
    }

    private List<GameObject> CreatePlatform(Random rand)
    {
        var platform = new Platform(new Vector3(3f, 14f, 3f));

        var maxHeight = Math.Max(_lastPosition.Y + rand.Next(1, 4), 8f);
        if (maxHeight >= 8f) maxHeight = rand.Next(-3, 0);

        var distance = Math.Max(_lastPosition.Z + rand.Next(5, 13), _lastPosition.Z + 10f);

        var pos = new Vector3(_isFirstPlatform ? 0 : _flipDirection ? rand.Next(2, 4) : rand.Next(-4, -2) , maxHeight, _isFirstPlatform ? 5f : distance);
        platform.GameObject.Transform.Position = pos;

        _lastPosition = platform.GameObject.Transform.Position;

        _isFirstPlatform = false;
        _flipDirection = !_flipDirection;

        return platform.GameObjects();
    }

    private List<GameObject> CreateMovingPlatform(Random rand)
    {
        var platform = new Platform(new Vector3(3f, 14f, 3f));

        var maxHeight = Math.Max(_lastPosition.Y + rand.Next(1, 4), 8f);
        if (maxHeight >= 8f) maxHeight = rand.Next(-3, 0);

        var distance = Math.Max(_lastPosition.Z + rand.Next(5, 13), _lastPosition.Z + 10f);
        
        var pos = new Vector3(_isFirstPlatform ? 0 : _flipDirection ? rand.Next(2, 4) : rand.Next(-4, -2), maxHeight, _isFirstPlatform ? 5f : distance);
        platform.GameObject.Transform.Position = pos;
        
        _lastPosition = platform.GameObject.Transform.Position;
        
        _isFirstPlatform = false;
        _flipDirection = !_flipDirection;

        var mPlatform = platform.AddScript<MovingPlatform>();
        mPlatform.MovingRange = new Vector3(_lastWasLava ? 0 : rand.Next(-6, 6), 0f, _lastWasLava ? -11.3f : 0);
        mPlatform.MovingSpeed = 0.4f;
        mPlatform.Player = OutOfBoundsRespawnScript.Player;

        _lastWasLava = false;

        return platform.GameObjects();
    }

    private List<GameObject> CreateLava(Random rand)
    {
        _lastWasLava = true;

        var maxHeight = Math.Max(_lastPosition.Y + rand.Next(1, 4), 8f);
        if (maxHeight >= 8f) maxHeight = rand.Next(-3, 0);

        var distance = Math.Max(_lastPosition.Z + rand.Next(5, 13), _lastPosition.Z + 10f);
        var pos = new Vector3(_flipDirection ? rand.Next(2, 4) : rand.Next(-4, -2), maxHeight, _isFirstPlatform ? 5f : distance);

        var lava = new Lava(new Vector3(5f, 14f, 5f), pos);
        var respawnScript = lava.AddScript<RespawnScript>();

        _lastPosition = lava.GameObject.Transform.Position;

        _flipDirection = !_flipDirection;
        return lava.GameObjects();
    }

    private List<GameObject> CreateSavePoint(Random rand, bool isLastSavePoint)
    {
        var maxHeight = Math.Max(_lastPosition.Y + rand.Next(1, 4), 8f);
        if (maxHeight >= 8f) maxHeight = rand.Next(-3, 0);

        var distance = Math.Max(_lastPosition.Z + rand.Next(5, 13), _lastPosition.Z + 10f);
        var pos = new Vector3(_flipDirection ? rand.Next(2, 4) : rand.Next(-4, -2), maxHeight, _isFirstPlatform ? 5f : distance);

        var platform = new SavePoint(pos, new Vector3(6f, 14f, 6f));
        var savePoint = platform.AddScript<SavePointScript>();
        savePoint.IsLastSavePoint = isLastSavePoint;

        _lastPosition = platform.SavePointObject.GameObject.Transform.Position;
        _flipDirection = !_flipDirection;

        return platform.GameObjects();
    }

    #endregion
}
