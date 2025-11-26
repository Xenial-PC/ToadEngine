using NAudio.Wave;
using OpenTK.Audio.OpenAL;

namespace ToadEngine.Classes.Base.Audio;

public class AudioManager
{
    public Dictionary<string, int> Sounds = new();

    public void Init()
    {
        ALBase.RegisterOpenALResolver();

        var devices = ALC.GetStringList(GetEnumerationStringList.DeviceSpecifier);
        var deviceName = ALC.GetString(ALDevice.Null, AlcGetString.DefaultDeviceSpecifier);
        foreach (var d in devices)
        {
            if (!d.Contains("OpenAL Soft")) continue;
            deviceName = d;
        }

        var device = ALC.OpenDevice(deviceName);
        var context = ALC.CreateContext(device, (int[])null!);
        ALC.MakeContextCurrent(context);
    }

    public int LoadSound(string file, string name)
    {
        if (Sounds.TryGetValue(name, out var sound)) return sound;
        if (!File.Exists(file)) return -1;

        var buffer = AL.GenBuffer();
        Sounds.TryAdd(name, buffer);

        byte[] pcmData;
        int channels;
        int sampleRate;
        int bitsPerSample;

        using (var audioReader = CreateReader(file))
        {
            channels = audioReader.WaveFormat.Channels;
            sampleRate = audioReader.WaveFormat.SampleRate;
            bitsPerSample = audioReader.WaveFormat.BitsPerSample;

            using var ms = new MemoryStream();
            audioReader.CopyTo(ms);
            pcmData = ms.ToArray();
        }

        var format = GetFormat(channels, bitsPerSample);
        AL.BufferData(buffer, format, pcmData, sampleRate);

        return buffer;
    }

    /// <summary>
    /// SoundFiles: (STR1)File, (STR2)Sound Name
    /// </summary>
    /// <param name="soundFiles"></param>
    public void LoadSounds(Dictionary<string, string> soundFiles)
    {
        foreach (var soundFile in soundFiles)
        {
            LoadSound(soundFile.Key, soundFile.Value);
        }
    }

    public int GetSound(string name)
    {
        Sounds.TryGetValue(name, out var soundId);
        return soundId;
    }

    public void SetListenerData(Vector3 position, Vector3 velocity = new())
    {
        AL.Listener(ALListener3f.Position, ref position);
        AL.Listener(ALListener3f.Velocity, ref velocity);
    }

    public void SetDistanceModel(ALDistanceModel model)
    {
        AL.DistanceModel(model);
    }

    public static ALFormat GetFormat(int channels, int bitsPerSample)
    {
        return channels switch
        {
            1 => bitsPerSample == 8 ? ALFormat.Mono8 : ALFormat.Mono16,
            2 => bitsPerSample == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16,
            _ => throw new NotSupportedException(
                $"The audio format with {channels} channels and {bitsPerSample} bits is not supported by OpenAL.")
        };
    }

    private WaveStream CreateReader(string file)
    {
        var ext = Path.GetExtension(file).ToLowerInvariant();
        return ext switch
        {
            ".wav" => new WaveFileReader(file),
            ".mp3" => new Mp3FileReader(file),
            ".aiff" => new AiffFileReader(file),
            _ => new AudioFileReader(file)
        };
    }

    public void Dispose()
    {
        foreach (var buffer in Sounds)
        {
            AL.DeleteBuffer(buffer.Value);
        }
    }
}
