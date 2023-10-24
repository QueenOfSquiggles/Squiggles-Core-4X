namespace Squiggles.Core.Data;

using System;
using Godot;
using Squiggles.Core.Events;

/// <summary>
/// The singleton which handles graphics options (including window options)
/// </summary>
public static class Graphics {

  //
  //  Meaningful information
  //      Defaults assigned as well
  //
  /// <summary>
  /// TGhe current <see cref="DisplayServer.WindowMode"/> enum value. It is dynamically defaults to Maxized (Windowed) in a DEBUG context, and fullscreen borderless in a release context. This option can then be modified in the options menu and applies to the window when requested. This enables users to also use fullscreen exclusive which is a method of forcing a synchronization of resolution and frame rate between the display and the program, but basically prevents use of other applications without glitching out on most (all??) operating systems.
  /// </summary>
  public static int Fullscreen { get; set; } =
#if DEBUG
      // In DEBUG builds I prefer windowed so I can see my debugging tools as well without much effort
      (int)DisplayServer.WindowMode.Maximized;
#else
        // fullscreen by default in release builds. Feels more AAA than trashy indie
        (int)DisplayServer.WindowMode.Fullscreen;
#endif

  /// <summary>
  /// Whether or not to render bloom (blurring of bright lights.)
  /// </summary>
  public static bool Bloom { get; set; } = true;
  /// <summary>
  /// Whether or not to render Screen Space Reflections (a cheap reflection effect that can only show what is already on screen)
  /// </summary>
  public static bool SSR { get; set; } = true;
  /// <summary>
  /// Whether or not to render Screen Space Ambient Occlusion (shadows where creases and borders of objects are)
  /// </summary>
  public static bool SSAO { get; set; } = true;
  /// <summary>
  /// Whether or not to render Screen Space Indiret Lighting. (Part of the new big render overhaul, it basically improved global illumination by attempted to guess what the indirect lighting contribution would be for the visible pixels)
  /// </summary>
  public static bool SSIL { get; set; } = true;
  /// <summary>
  /// Whether ot not to render with Signed Distance Field Global Illumination. Basically it does some heavy computations, but it makes your scenes significantly more realistic.
  /// </summary>
  public static bool SDFGI { get; set; } = true;
  /// <summary>
  /// The amount of exposure to use for the current tonemap. This defaults to 1.0. Higher exposure levels will brighten dark scenes and lower levels will darken bright levels.
  /// </summary>
  public static float TonemapExposure { get; set; } = 1.0f;
  /// <summary>
  /// The straight brightness adjustment. Use sparingly
  /// </summary>
  public static float Brightness { get; set; } = 1.0f;
  /// <summary>
  /// The straight Contrast adjustment. Use sparingly
  /// </summary>
  public static float Contrast { get; set; } = 1.0f;
  /// <summary>
  /// The gay (why they gotta be straight???) saturation adjustment. Use sparingly
  /// </summary>
  public static float Saturation { get; set; } = 1.0f;

  //
  //  Callback
  //

  /// <summary>
  /// An event which triggers whenever the graphics settings are changed. Detected through calls to <see cref="MarkGraphicsChanged"/> to prevent severe stuttering when many values are changed
  /// </summary>
  public static event Action OnGraphicsSettingsChanged;

  private const string FILE_PATH = "graphics.json";

  public static void Load() {
    EventBus.Data.SerializeAll += SaveSettings;
    var builder = new SaveDataBuilder(FILE_PATH, useCurrentSaveSlot: false).LoadFromFile();
    Fullscreen = builder.GetInt(nameof(Fullscreen), out var i1) ? i1 : Fullscreen;
    Bloom = builder.GetBool(nameof(Bloom), out var b1) ? b1 : Bloom;
    SSR = builder.GetBool(nameof(SSR), out var b2) ? b2 : SSR;
    SSAO = builder.GetBool(nameof(SSAO), out var b3) ? b3 : SSAO;
    SSIL = builder.GetBool(nameof(SSIL), out var b4) ? b4 : SSIL;
    SDFGI = builder.GetBool(nameof(SDFGI), out var b5) ? b5 : SDFGI;
    TonemapExposure = builder.GetFloat(nameof(TonemapExposure), out var f1) ? f1 : TonemapExposure;
    Brightness = builder.GetFloat(nameof(Brightness), out var f2) ? f2 : Brightness;
    Contrast = builder.GetFloat(nameof(Contrast), out var f3) ? f3 : Contrast;
    Saturation = builder.GetFloat(nameof(Saturation), out var f4) ? f4 : Saturation;
    MarkGraphicsChanged();
  }

  public static void SaveSettings() {
    var builder = new SaveDataBuilder(FILE_PATH, useCurrentSaveSlot: false);

    builder.PutInt(nameof(Fullscreen), Fullscreen);
    builder.PutBool(nameof(Bloom), Bloom);
    builder.PutBool(nameof(SSR), SSR);
    builder.PutBool(nameof(SSAO), SSAO);
    builder.PutBool(nameof(SSIL), SSIL);
    builder.PutBool(nameof(SDFGI), SDFGI);
    builder.PutFloat(nameof(TonemapExposure), TonemapExposure);
    builder.PutFloat(nameof(Brightness), Brightness);
    builder.PutFloat(nameof(Contrast), Contrast);
    builder.PutFloat(nameof(Saturation), Saturation);

    builder.SaveToFile();
  }



  /// <summary>
  /// Marks the graphics as changed, which allows this singleton to handle everything that has changed at once instead of after each individual property.
  /// </summary>
  public static void MarkGraphicsChanged() {
    OnGraphicsSettingsChanged?.Invoke();
    var win_mode = (int)DisplayServer.WindowGetMode();
    if (win_mode != Fullscreen) {
      DisplayServer.WindowSetMode((DisplayServer.WindowMode)Fullscreen);
    }
    SaveSettings();
  }
}
