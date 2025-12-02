namespace ToadEngine.Classes.Base.Scripting.Base;

public class Time
{
    private static float _deltaTime, _timeScale = 100f;
    private static readonly DateTime _startTime = DateTime.Now;

    /// <summary>
    /// Scaled DeltaTime shows the scaled time in between frames
    /// </summary>
    public static float DeltaTime
    {
        get => _deltaTime * TimeScale;
        set => _deltaTime = value;
    }

    /// <summary>
    /// The Unscaled version of deltaTime
    /// </summary>
    public static float UDeltaTime => _deltaTime;

    /// <summary>
    /// Time since application startup, in seconds
    /// </summary>
    public static float TimeSinceStartup => (DateTime.Now - _startTime).Seconds;

    /// <summary>
    /// Fixed scaled time step updates on specific intervals not using deltaTime 
    /// </summary>
    public static float FixedDeltaTime => FixedTime * TimeScale;

    /// <summary>
    /// Scales everything moving while using DeltaTime
    /// </summary>
    public static float TimeScale { get => _timeScale / 100f; set => _timeScale = value; }

    /// <summary>
    /// Fixed delta time used for physics update ticks (non-scaled)
    /// </summary>
    public static float FixedTime = 1f / 60f;

    /// <summary>
    /// Accumulated time accounts for time in between updates for fixed time (scaled)
    /// </summary>
    public static float AccumulatedTime;
}
