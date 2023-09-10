using System;
using Godot;

namespace queen.data;

public class AudioBuses
{

    //
    //  Meaningful information
    //      Defaults assigned as well
    //

    public float[] Volumes = new float[0];
    public float VolumeMain = 0.0f;
    public float VolumeVO = 0.0f;
    public float VolumeSFX = 0.0f;
    public float VolumeCreature = 0.0f;
    public float VolumeDrones = 0.0f;


    //
    //  Singleton Setup
    //
    public static AudioBuses Instance
    {
        get
        {
            if (_instance == null) CreateInstance();
            return _instance;

        }
    }
    private static AudioBuses _instance = null;
    private const string FILE_PATH = "audio.json";

    private static void CreateInstance()
    {
        _instance = new AudioBuses();
        var loaded = Data.Load<AudioBuses>(FILE_PATH);
        if (loaded != null) _instance = loaded;
        UpdateAudioServer();
    }

    public static void SaveSettings()
    {
        if (_instance == null) return;
        UpdateAudioServer();
        Data.Save(_instance, FILE_PATH);
    }

    private static void UpdateAudioServer()
    {
        if (_instance.Volumes.Length != AudioServer.BusCount)
        {
            Array.Resize(ref _instance.Volumes, AudioServer.BusCount);
        }
        for (int i = 0; i < AudioServer.BusCount; i++)
        {
            AudioServer.SetBusVolumeDb(i, _instance.Volumes[i]);
        }
    }

}