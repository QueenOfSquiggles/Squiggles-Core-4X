using Godot;
using queen.error;

namespace queen.data;

public class Access
{


    public const int FONT_GOTHIC = 0;
    public const int FONT_NOTO_SANS = 1;
    public const int FONT_OPEN_DYSLEXIE = 2;
    private const string FONT_PATH_GOTHIC = "res://Core/Assets/Fonts/DelaGothicOne-Regular.ttf";
    private const string FONT_PATH_NOTO_SANS = "res://Core/Assets/Fonts/NotoSans-Regular.ttf";
    private const string FONT_PATH_OPEN_DYSLEXIE = "res://Core/Assets/Fonts/OpenDyslexic-Regular.otf";

    public const float RETICLE_SHOW = 0.4f;
    public const float RETICLE_HIDE_VISIBLE = 0.1f;
    public const float RETICLE_HIDE_INVISIBLE = 0.0f;

    //
    //  Meaningful information
    //      Defaults assigned as well
    //
    public bool UseSubtitles = true;
    public bool PreventFlashingLights = false;
    public float AudioDecibelLimit = 0.0f;
    public int FontOption = FONT_GOTHIC;
    public float EngineTimeScale = 1.0f;
    public bool ReadVisibleTextAloud = false;
    public float GUI_Scale = 1.0f;
    public float ReticleShownScale = RETICLE_SHOW; // currently useless. Maybe I'll change this in the future?
    public float ReticleHiddenScale = RETICLE_HIDE_INVISIBLE;
    public bool AlwaysShowReticle = false;



    //
    //  Singleton Setup
    //
    public static Access Instance
    {
        get
        {
            if (_instance is null) CreateInstance();
            return _instance;
        }
    }
    private static Access _instance = null;
    private const string FILE_PATH = "access.json";

    public static void ForceLoadInstance()
    {
        if (_instance != null) return;
        CreateInstance();
    }

    private static void CreateInstance()
    {
        _instance = new Access();
        var loaded = Data.Load<Access>(FILE_PATH);
        if (loaded is not null)
        {
            _instance = loaded;
            ApplyChanges();
        }
    }

    public static void SaveSettings()
    {
        if (_instance == null) return;
        ApplyChanges();
        Data.Save(_instance, FILE_PATH);
    }

    private static void ApplyChanges()
    {
        // audio limiter
        var effect_count = AudioServer.GetBusEffectCount(0);
        AudioEffectLimiter ael = null;
        for (int i = 0; i < effect_count; i++)
        {
            ael = AudioServer.GetBusEffect(0, i) as AudioEffectLimiter;
            if (ael is null) continue;
            break;
        }
        bool add_effect = false;
        if (ael is null)
        {
            // no limiter effect found
            add_effect = true;
            ael = new();
        }
        ael.CeilingDb = _instance.AudioDecibelLimit;
        if (add_effect) AudioServer.AddBusEffect(0, ael);

        // font management
        string path = "";
        switch (_instance.FontOption)
        {
            case FONT_GOTHIC:
                path = FONT_PATH_GOTHIC;
                break;
            case FONT_NOTO_SANS:
                path = FONT_PATH_NOTO_SANS;
                break;
            case FONT_OPEN_DYSLEXIE:
                path = FONT_PATH_OPEN_DYSLEXIE;
                break;
        }
        var font = GD.Load<FontFile>(path);
        if (font == null) Print.Error($"Failed to load font option from file: {path}");
        else ThemeDB.GetProjectTheme().DefaultFont = font;

        ThemeDB.FallbackBaseScale = Instance.GUI_Scale;

        // Time scale
        Engine.TimeScale = Instance.EngineTimeScale;

        Instance.ReticleHiddenScale = Instance.AlwaysShowReticle ? RETICLE_HIDE_VISIBLE : RETICLE_HIDE_INVISIBLE;
    }
}