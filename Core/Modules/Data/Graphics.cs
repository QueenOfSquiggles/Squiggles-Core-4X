namespace Squiggles.Core.Data;

using System;
using Godot;
using Squiggles.Core.Events;

/// <summary>
/// The singleton which handles graphics options (including window options)
/// </summary>
public class Graphics {

  //
  //  Meaningful information
  //      Defaults assigned as well
  //
  /// <summary>
  /// TGhe current <see cref="DisplayServer.WindowMode"/> enum value. It is dynamically defaults to Maxized (Windowed) in a DEBUG context, and fullscreen borderless in a release context. This option can then be modified in the options menu and applies to the window when requested. This enables users to also use fullscreen exclusive which is a method of forcing a synchronization of resolution and frame rate between the display and the program, but basically prevents use of other applications without glitching out on most (all??) operating systems.
  /// </summary>
  public int Fullscreen =
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
  public bool Bloom = true;
  /// <summary>
  /// Whether or not to render Screen Space Reflections (a cheap reflection effect that can only show what is already on screen)
  /// </summary>
  public bool SSR = true;
  /// <summary>
  /// Whether or not to render Screen Space Ambient Occlusion (shadows where creases and borders of objects are)
  /// </summary>
  public bool SSAO = true;
  /// <summary>
  /// Whether or not to render Screen Space Indiret Lighting. (Part of the new big render overhaul, it basically improved global illumination by attempted to guess what the indirect lighting contribution would be for the visible pixels)
  /// </summary>
  public bool SSIL;
  /// <summary>
  /// Whether ot not to render with Signed Distance Field Global Illumination. Basically it does some heavy computations, but it makes your scenes significantly more realistic.
  /// </summary>
  public bool SDFGI = true;
  /// <summary>
  /// The amount of exposure to use for the current tonemap. This defaults to 1.0. Higher exposure levels will brighten dark scenes and lower levels will darken bright levels.
  /// </summary>
  public float TonemapExposure = 1.0f;
  /// <summary>
  /// The straight brightness adjustment. Use sparingly
  /// </summary>
  public float Brightness = 1.0f;
  /// <summary>
  /// The straight Contrast adjustment. Use sparingly
  /// </summary>
  public float Contrast = 1.0f;
  /// <summary>
  /// The gay (why they gotta be straight???) saturation adjustment. Use sparingly
  /// </summary>
  public float Saturation = 1.0f;

  //
  //  Callback
  //

  /// <summary>
  /// An event which triggers whenever the graphics settings are changed. Detected through calls to <see cref="MarkGraphicsChanged"/> to prevent severe stuttering when many values are changed
  /// </summary>
  public event Action OnGraphicsSettingsChanged;

  //
  //  Singleton Setup
  //
  public static Graphics Instance {
    get {
      if (_instance is null) {
        CreateInstance();
      }

      return _instance;

    }
  }
  private static Graphics _instance;
  private const string FILE_PATH = "graphics.json";

  private static void CreateInstance() {
    _instance = new Graphics();
    var loaded = SaveData.Load<Graphics>(FILE_PATH);
    if (loaded != null) {
      _instance = loaded;
    }
    else {
      SaveSettings();
    }
    EventBus.Data.SerializeAll += SaveSettings;

    DisplayServer.WindowSetMode((DisplayServer.WindowMode)_instance.Fullscreen);
  }

  public static void SaveSettings() {
    if (_instance == null) {
      return;
    }

    SaveData.Save(_instance, FILE_PATH);
  }

  /// <summary>
  /// Marks the graphics as changed, which allows this singleton to handle everything that has changed at once instead of after each individual property.
  /// </summary>
  public static void MarkGraphicsChanged() {
    Instance.OnGraphicsSettingsChanged?.Invoke();
    var win_mode = (int)DisplayServer.WindowGetMode();
    if (win_mode != Instance.Fullscreen) {
      DisplayServer.WindowSetMode((DisplayServer.WindowMode)Instance.Fullscreen);
    }

    SaveSettings();
  }
}
