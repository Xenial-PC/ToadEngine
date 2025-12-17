using SimplePlatformer.Classes.GameObjects.Controllers;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.GameObjects.Scripts;

public class MovingPlatform : MonoBehavior
{
    public Vector3 MovingRange;
    public float MovingSpeed = 0.5f;

    public Player Player = null!;
    public FPController Controller = null!; 

    private bool _isPlayerOnTop, _isForward = true;
    private Vector3 _originalPosition, _lastPlatformPosition;
    private float _t;

    public void Start()
    {
        _originalPosition = GameObject.Transform.Position;
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

        GameObject.Transform.Position = _originalPosition + (MovingRange * _t);

        if (_isPlayerOnTop)
        {
            var delta = (GameObject.Transform.Position - _lastPlatformPosition);
            Controller.Body.Pose.Position += (System.Numerics.Vector3)delta;
        }

        _lastPlatformPosition = GameObject.Transform.Position;
    }

    public void OnTriggerEnter(GameObject other)
    {
        if (other.GetComponent<FPController>() != null)
            _isPlayerOnTop = true;
    }

    public void OnTriggerExit(GameObject other)
    {
        if (other.GetComponent<FPController>() != null)
            _isPlayerOnTop = false;
    }
}
