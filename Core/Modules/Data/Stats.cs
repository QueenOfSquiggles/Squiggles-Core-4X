using Godot;
using queen.error;

namespace queen.data;

public class Stats
{

    //
    //  Meaningful information
    //      Defaults assigned as well
    //
    public bool FirstTimeLaunch = true;
    public float TotalPlayTime = 0.0f;




    //
    //  Singleton Setup
    //
    public static Stats Instance
    {
        get
        {
            if (_instance == null) CreateInstance();
            return _instance;

        }
    }
    private static Stats _instance = null;
    private const string FILE_PATH = "stats.json";

    public static void ForceLoadInstance()
    {
        if (_instance != null) return;
        CreateInstance();
    }

    private static void CreateInstance()
    {
        _instance = new Stats();
        var loaded = Data.Load<Stats>(FILE_PATH);
        if (loaded != null) _instance = loaded;
    }

    public static void SaveSettings()
    {
        if (_instance == null) return;
        Data.Save(_instance, FILE_PATH);
    }

}