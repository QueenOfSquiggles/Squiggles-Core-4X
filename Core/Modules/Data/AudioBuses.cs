using Godot;

namespace queen.data;

public class AudioBuses
{

    //
    //  Meaningful information
    //      Defaults assigned as well
    //

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
        AudioServer.SetBusVolumeDb(0, _instance.VolumeMain);
        AudioServer.SetBusVolumeDb(1, _instance.VolumeVO);
        AudioServer.SetBusVolumeDb(2, _instance.VolumeSFX);
        AudioServer.SetBusVolumeDb(3, _instance.VolumeCreature);
        AudioServer.SetBusVolumeDb(4, _instance.VolumeDrones);
    }

}