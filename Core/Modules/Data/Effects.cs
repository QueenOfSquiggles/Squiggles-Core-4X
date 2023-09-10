using System;
using Godot;
using queen.error;

namespace queen.data;

public class Effects
{

    //
    //  Meaningful information
    //      Defaults assigned as well
    //

    public float RumbleStrength = 0.75f;
    public float MaxRumbleDuration = 5.0f;
    public float ScreenShakeStrength = 0.75f;
    public float MaxScreenShakeDuration = 10.0f;

    //
    //  Functions for applying effects
    //

    public event Action<float, float, float> RequestScreenShake;

    public static void Shake(SceneTree tree_access, float speed, float strength, float duration)
    {
        var str = strength * Instance.ScreenShakeStrength;
        var dur = Mathf.Clamp(duration, 0.0f, Instance.MaxScreenShakeDuration);
        Instance.RequestScreenShake?.Invoke(speed, str, 1.0f / dur);
    }

    public static void Rumble(float strong, float weak, float duration = 0.1f, int controller_id = -1)
    {
        var current = Input.GetConnectedJoypads();
        var len = duration * Instance.MaxRumbleDuration;
        var str = Mathf.Clamp(strong * Instance.RumbleStrength, 0.0f, 1.0f);
        var wee = Mathf.Clamp(weak * Instance.RumbleStrength, 0.0f, 1.0f);

        if (controller_id < 0)
        {
            foreach (var index in current)
                Input.StartJoyVibration(index, str, wee, len);
        }
        else
        {
            Input.StartJoyVibration(controller_id, str, wee, len);
        }
    }

    //
    //  Singleton Setup
    //
    public static Effects Instance
    {
        get
        {
            if (_instance == null) CreateInstance();
            return _instance;

        }
    }
    private static Effects _instance = null;
    private const string FILE_PATH = "effects.json";

    private static void CreateInstance()
    {
        _instance = new Effects();
        var loaded = Data.Load<Effects>(FILE_PATH);
        if (loaded != null) _instance = loaded;
    }

    public static void SaveSettings()
    {
        if (_instance == null) return;
        Data.Save(_instance, FILE_PATH);
    }
}