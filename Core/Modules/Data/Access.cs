namespace Squiggles.Core.Data;
using Godot;
using Squiggles.Core.Error;

public class Access {


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
  public bool PreventFlashingLights;
  public float AudioDecibelLimit;
  public int FontOption = FONT_GOTHIC;
  public float EngineTimeScale = 1.0f;
  public bool ReadVisibleTextAloud;
  public float GUI_Scale = 1.0f;
  public float ReticleShownScale = RETICLE_SHOW; // currently useless. Maybe I'll change this in the future?
  public float ReticleHiddenScale = RETICLE_HIDE_INVISIBLE;
  public bool AlwaysShowReticle;



  //
  //  Singleton Setup
  //
  public static Access Instance {
    get {
      if (_instance is null) {
        CreateInstance();
      }

      return _instance;
    }
  }
  private static Access _instance;
  private const string FILE_PATH = "access.json";

  public static void ForceLoadInstance() {
    if (_instance != null) {
      return;
    }

    CreateInstance();
  }

  private static void CreateInstance() {
    _instance = new Access();
    var loaded = SaveData.Load<Access>(FILE_PATH);
    if (loaded is not null) {
      _instance = loaded;
      ApplyChanges();
    }
  }

  public static void SaveSettings() {
    if (_instance == null) {
      return;
    }

    ApplyChanges();
    SaveData.Save(_instance, FILE_PATH);
  }

  private static void ApplyChanges() {
    // audio limiter
    var effect_count = AudioServer.GetBusEffectCount(0);
    AudioEffectLimiter ael = null;
    for (var i = 0; i < effect_count; i++) {
      ael = AudioServer.GetBusEffect(0, i) as AudioEffectLimiter;
      if (ael is null) {
        continue;
      }

      break;
    }
    var add_effect = false;
    if (ael is null) {
      // no limiter effect found
      add_effect = true;
      ael = new();
    }
    ael.CeilingDb = _instance.AudioDecibelLimit;
    if (add_effect) {
      AudioServer.AddBusEffect(0, ael);
    }

    // font management
    var path = "";
    switch (_instance.FontOption) {
      case FONT_GOTHIC:
        path = FONT_PATH_GOTHIC;
        break;
      case FONT_NOTO_SANS:
        path = FONT_PATH_NOTO_SANS;
        break;
      case FONT_OPEN_DYSLEXIE:
        path = FONT_PATH_OPEN_DYSLEXIE;
        break;
      default:
        break;
    }
    var font = GD.Load<FontFile>(path);
    if (font == null) {
      Print.Error($"Failed to load font option from file: {path}");
    }
    else {
      ThemeDB.GetProjectTheme().DefaultFont = font;
    }

    ThemeDB.FallbackBaseScale = Instance.GUI_Scale;

    // Time scale
    Engine.TimeScale = Instance.EngineTimeScale;

    Instance.ReticleHiddenScale = Instance.AlwaysShowReticle ? RETICLE_HIDE_VISIBLE : RETICLE_HIDE_INVISIBLE;
  }
}
