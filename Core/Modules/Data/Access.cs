namespace Squiggles.Core.Data;
using Godot;
using Squiggles.Core.Error;
using Squiggles.Core.Events;

/// <summary>
/// The singleton for managing accessibility features
/// </summary>
public class Access {


  /// <summary>
  /// enum proxy constant for the "DelaGoticOne" font
  /// </summary>
  public const int FONT_GOTHIC = 0;
  /// <summary>
  /// enum proxy constant for the "NotoSans-Regular" font
  /// </summary>
  public const int FONT_NOTO_SANS = 1;
  /// <summary>
  /// enum proxy constant for the "OpenDyslexic-Regular" font
  /// </summary>
  public const int FONT_OPEN_DYSLEXIE = 2;
  private const string FONT_PATH_GOTHIC = "res://Core/Assets/Fonts/DelaGothicOne-Regular.ttf";
  private const string FONT_PATH_NOTO_SANS = "res://Core/Assets/Fonts/NotoSans-Regular.ttf";
  private const string FONT_PATH_OPEN_DYSLEXIE = "res://Core/Assets/Fonts/OpenDyslexic-Regular.otf";

  /// <summary>
  /// The scale at which to show the reticle when "shown"
  /// </summary>
  public const float RETICLE_SHOW = 0.4f;
  /// <summary>
  /// The scale at which to show the reticle when "hidden" but <see cref="AlwaysShowReticle"/> is true.
  /// </summary>
  public const float RETICLE_HIDE_VISIBLE = 0.1f;
  /// <summary>
  /// The scale at which to show the reticle when "hidden" and <see cref="AlwaysShowReticle"/> is false. Used for dynamic tweening but should stick to 0.0
  /// </summary>
  public const float RETICLE_HIDE_INVISIBLE = 0.0f;

  //
  //  Meaningful information
  //      Defaults assigned as well
  //
  /// <summary>
  /// Whether or not to show subtitles
  /// </summary>
  public bool UseSubtitles = true;
  /// <summary>
  /// Whether or not to prevent flashing lights (relies on developer implementation)
  /// </summary>
  public bool PreventFlashingLights;
  /// <summary>
  /// An audio decibel limit which is applied dynamically to the Audio Bus to prevent sounds exceeding a particular volume. Useful for those with auditory sensitivities.
  /// </summary>
  public float AudioDecibelLimit;
  /// <summary>
  /// The preferred font of the user. Defaults to Gothic for style, but can be changed for clarity. Provided no overriding theme resource is applied, this will affect all GUI elements (even custom ones) thank's to Godot's custom theme project setting.
  /// </summary>
  public int FontOption = FONT_GOTHIC;
  /// <summary>
  /// A time scale to change how fast the engine processes. This can be helpful for those with slower reaction times, but is a rather naive solution.
  /// </summary>
  public float EngineTimeScale = 1.0f;
  /// <summary>
  /// An in-dev approach to enabling TTS for GUI elements. It's currently not super good and mainly awaiting further development.
  /// </summary>
  public bool ReadVisibleTextAloud;
  /// <summary>
  /// An in-dev attempt at scaling the GUI. Doesn't seem to work too well. Waiting on further development insight.
  /// </summary>
  public float GUI_Scale = 1.0f;
  /// <summary>
  /// The current scale at which the reticle is shown when considered "visible"
  /// </summary>
  public float ReticleShownScale = RETICLE_SHOW; // currently useless. Maybe I'll change this in the future?
  /// <summary>
  /// The current scale at which the reticle is shown when considered "hidden"
  /// </summary>
  public float ReticleHiddenScale = RETICLE_HIDE_INVISIBLE;
  /// <summary>
  /// An option to always show the reticle or not. This could potentially help with motion sickness in some individuals for playing first person games/
  /// </summary>
  public bool AlwaysShowReticle;



  //
  //  Singleton Setup
  //
  /// <summary>
  /// The instance to access for this singleton.
  /// </summary>
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

  /// <summary>
  ///  Forces an instance to be loaded. Helpful when these properties are required early on in the app's life cycle.
  /// </summary>
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

  /// <summary>
  /// Saves the current settings out to disk (global scope)
  /// </summary>
  public static void SaveSettings() {
    if (_instance == null) {
      return;
    }
    EventBus.Data.SerializeAll += SaveSettings;

    ApplyChanges();
    SaveData.Save(_instance, FILE_PATH);
  }

  /// <summary>
  /// Causes changes to various systems within Godot to properly respect the settings assigned (declarative approach)
  /// </summary>
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

    // GUI scale
    ThemeDB.FallbackBaseScale = Instance.GUI_Scale;

    // Time scale
    Engine.TimeScale = Instance.EngineTimeScale;

    Instance.ReticleHiddenScale = Instance.AlwaysShowReticle ? RETICLE_HIDE_VISIBLE : RETICLE_HIDE_INVISIBLE;
  }
}
