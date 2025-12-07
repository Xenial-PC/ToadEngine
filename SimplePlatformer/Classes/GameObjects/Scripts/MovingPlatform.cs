using SimplePlatformer.Classes.GameObjects.Controllers;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.GameObjects.Scripts;

public class MovingPlatform : Behavior
{
    public Vector3 MovingRange;
    public float MovingSpeed = 0.5f;

    public FPController Player = null!;

    private bool _isPlayerOnTop, _isForward = true;
    private Vector3 _originalPosition, _lastPlatformPosition;
    private float _t;

    public void Start()
    {
        _originalPosition = GameObject.Parent.Transform.Position;
    }

    public void Update()
    {
        if (_isForward) _t += MovingSpeed * Time.DeltaTime;
        else _t -= MovingSpeed * Time.DeltaTime;

        switch (_t)
        {
            case >= 1f:
                _t = 1f;
                _isForward = false;
                break;
            case <= 0f:
                _t = 0f;
                _isForward = true;
                break;
        }

        GameObject.Parent.Transform.Position = _originalPosition + (MovingRange * _t);

        if (_isPlayerOnTop)
        {
            var delta = (GameObject.Parent.Transform.Position - _lastPlatformPosition);
            Player.Controller.Body.Pose.Position += (System.Numerics.Vector3)delta;
        }

        _lastPlatformPosition = GameObject.Parent.Transform.Position;
    }

    public void OnTriggerEnter(GameObject other)
    {
        if (other.GetComponent<FPController.FPControllerScript>() != null)
            _isPlayerOnTop = true;
    }

    public void OnTriggerExit(GameObject other)
    {
        if (other.GetComponent<FPController.FPControllerScript>() != null)
            _isPlayerOnTop = false;
    }
}
