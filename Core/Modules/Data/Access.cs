namespace Squiggles.Core.Data;

using Godot;
using Squiggles.Core.Error;
using Squiggles.Core.Events;

/// <summary>
/// The singleton for managing accessibility features
/// </summary>
public static class Access {


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
  public static bool UseSubtitles { get; set; } = true;
  /// <summary>
  /// Whether or not to prevent flashing lights (relies on developer implementation)
  /// </summary>
  public static bool PreventFlashingLights { get; set; }
  /// <summary>
  /// An audio decibel limit which is applied dynamically to the Audio Bus to prevent sounds exceeding a particular volume. Useful for those with auditory sensitivities.
  /// </summary>
  public static float AudioDecibelLimit { get; set; }
  /// <summary>
  /// The preferred font of the user. Defaults to Gothic for style, but can be changed for clarity. Provided no overriding theme resource is applied, this will affect all GUI elements (even custom ones) thank's to Godot's custom theme project setting.
  /// </summary>
  public static int FontOption { get; set; } = FONT_GOTHIC;
  /// <summary>
  /// A time scale to change how fast the engine processes. This can be helpful for those with slower reaction times, but is a rather naive solution.
  /// </summary>
  public static float EngineTimeScale { get; set; } = 1.0f;
  /// <summary>
  /// An in-dev approach to enabling TTS for GUI elements. It's currently not super good and mainly awaiting further development.
  /// </summary>
  public static bool ReadVisibleTextAloud { get; set; }
  /// <summary>
  /// An in-dev attempt at scaling the GUI. Doesn't seem to work too well. Waiting on further development insight.
  /// </summary>
  public static float GUI_Scale { get; set; } = 1.0f;
  /// <summary>
  /// The current scale at which the reticle is shown when considered "visible"
  /// </summary>
  public static float ReticleShownScale { get; set; } = RETICLE_SHOW; // currently useless. Maybe I'll change this in the future?
  /// <summary>
  /// The current scale at which the reticle is shown when considered "hidden"
  /// </summary>
  public static float ReticleHiddenScale { get; set; } = RETICLE_HIDE_INVISIBLE;
  /// <summary>
  /// An option to always show the reticle or not. This could potentially help with motion sickness in some individuals for playing first person games/
  /// </summary>
  public static bool AlwaysShowReticle { get; set; }


  private const string FILE_PATH = "access.json";

  public static void Load() {
    var builder = new SaveDataBuilder(FILE_PATH, useCurrentSaveSlot: false).LoadFromFile();
    // bools
    UseSubtitles = builder.GetBool(nameof(UseSubtitles), out var v1) ? v1 : UseSubtitles;
    PreventFlashingLights = builder.GetBool(nameof(PreventFlashingLights), out var v2) ? v2 : PreventFlashingLights;
    ReadVisibleTextAloud = builder.GetBool(nameof(ReadVisibleTextAloud), out var v3) ? v3 : ReadVisibleTextAloud;
    AlwaysShowReticle = builder.GetBool(nameof(AlwaysShowReticle), out var v4) ? v4 : AlwaysShowReticle;

    // floats
    AudioDecibelLimit = builder.GetFloat(nameof(AudioDecibelLimit), out var f1) ? f1 : AudioDecibelLimit;
    EngineTimeScale = builder.GetFloat(nameof(EngineTimeScale), out var f2) ? f2 : EngineTimeScale;
    GUI_Scale = builder.GetFloat(nameof(GUI_Scale), out var f3) ? f3 : GUI_Scale;
    ReticleShownScale = builder.GetFloat(nameof(ReticleShownScale), out var f4) ? f4 : ReticleShownScale;
    ReticleHiddenScale = builder.GetFloat(nameof(ReticleHiddenScale), out var f5) ? f5 : ReticleHiddenScale;

    // ints
    FontOption = builder.GetInt(nameof(FontOption), out var i1) ? i1 : FontOption;
    ApplyChanges();
  }

  /// <summary>
  /// Saves the current settings out to disk (global scope)
  /// </summary>
  public static void SaveSettings() {
    EventBus.Data.SerializeAll += SaveSettings;
    ApplyChanges();

    var builder = new SaveDataBuilder(FILE_PATH, useCurrentSaveSlot: false);

    // bools
    builder.PutBool(nameof(UseSubtitles), UseSubtitles);
    builder.PutBool(nameof(PreventFlashingLights), PreventFlashingLights);
    builder.PutBool(nameof(ReadVisibleTextAloud), ReadVisibleTextAloud);
    builder.PutBool(nameof(AlwaysShowReticle), AlwaysShowReticle);

    // floats
    builder.PutFloat(nameof(AudioDecibelLimit), AudioDecibelLimit);
    builder.PutFloat(nameof(EngineTimeScale), EngineTimeScale);
    builder.PutFloat(nameof(GUI_Scale), GUI_Scale);
    builder.PutFloat(nameof(ReticleShownScale), ReticleShownScale);
    builder.PutFloat(nameof(ReticleHiddenScale), ReticleHiddenScale);

    // ints
    builder.PutInt(nameof(FontOption), FontOption);

    builder.SaveToFile();
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
    ael.CeilingDb = AudioDecibelLimit;
    if (add_effect) {
      AudioServer.AddBusEffect(0, ael);
    }

    // font management
    var path = "";
    switch (FontOption) {
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
    ThemeDB.FallbackBaseScale = GUI_Scale;

    // Time scale
    Engine.TimeScale = EngineTimeScale;

    ReticleHiddenScale = AlwaysShowReticle ? RETICLE_HIDE_VISIBLE : RETICLE_HIDE_INVISIBLE;
  }
}
