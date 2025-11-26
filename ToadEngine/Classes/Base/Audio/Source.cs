using OpenTK.Audio.OpenAL;
using System.Drawing.Printing;

namespace ToadEngine.Classes.Base.Audio;

public class Source
{
    public int SourceId = AL.GenSource();
    private float _timeStep;

    public Source()
    {
        SetupSource();
    }

    public void Play(int buffer)
    {
        Stop();
        AL.Source(SourceId, ALSourcei.Buffer, buffer);
        Play();
    }

    public void Play(int buffer, float interval, float timer)
    {
        _timeStep -= timer;
        if (!(_timeStep <= 0f)) return;

        Stop();
        AL.Source(SourceId, ALSourcei.Buffer, buffer);
        Play();

        _timeStep = interval;
    }

    public void Pause()
    {
        AL.SourcePause(SourceId);
    }

    public void Play()
    {
        AL.SourcePlay(SourceId);
    }

    public void Stop()
    {
        AL.SourceStop(SourceId);
    }

    public void SetVelocity(Vector3 velocity)
    {
        AL.Source(SourceId, ALSource3f.Velocity, ref velocity);
    }

    public void SetLooping(bool loop)
    {
        AL.Source(SourceId, ALSourceb.Looping, loop);
    }

    public void SetupSource(float rollOffFactor = 4f, float referenceDistance = 2f, float maxDistance = 30f)
    {
        AL.Source(SourceId, ALSourcef.RolloffFactor, rollOffFactor);
        AL.Source(SourceId, ALSourcef.ReferenceDistance, referenceDistance);
        AL.Source(SourceId, ALSourcef.MaxDistance, maxDistance);
    }

    public bool IsPlaying()
    {
        return AL.GetSource(SourceId, ALGetSourcei.SourceState) == (int)ALSourceState.Playing;
    }

    public void SetVolume(float volume)
    {
        AL.Source(SourceId, ALSourcef.Gain, volume);
    }

    public void SetPitch(float pitch)
    {
        AL.Source(SourceId, ALSourcef.Pitch, pitch);
    }

    public void SetPosition(Vector3 position)
    {
        AL.Source(SourceId, ALSource3f.Position, ref position);
    }

    public void Dispose()
    {
        AL.DeleteSource(SourceId);
    }
}
