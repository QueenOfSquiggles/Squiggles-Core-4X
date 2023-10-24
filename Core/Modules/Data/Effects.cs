namespace Squiggles.Core.Data;

using System;
using Godot;
using Squiggles.Core.Events;

/// <summary>
/// A singleton for managing juicy effects. Also handles the settings for reducing discomfort from certain effects. i.e. a player can disable screen shake if it affects them.
/// </summary>
public static class Effects {

  //
  //  Meaningful information
  //      Defaults assigned as well
  //

  /// <summary>
  /// The default strength at which to rumble controllers at
  /// </summary>
  public static float RumbleStrength { get; set; } = 0.75f;
  /// <summary>
  /// The maximum rumble allowed for rumbling controllers (clamped)
  /// </summary>
  public static float MaxRumbleDuration { get; set; } = 5.0f;
  /// <summary>
  /// The default strength at which the screen shakes
  /// </summary>
  public static float ScreenShakeStrength { get; set; } = 0.75f;
  /// <summary>
  /// The maximum amount of screen shake allowed.
  /// </summary>
  public static float MaxScreenShakeDuration { get; set; } = 10.0f;

  //
  //  Functions for applying effects
  //

  /// <summary>
  /// Event for when screen shake is requested. Parameters are: speed, strength, duration.
  /// Strength and Duration are adjusted to meet the settings of the user.
  /// </summary>
  public static event Action<float, float, float> RequestScreenShake;

  /// <summary>
  /// Triggers a screen shake
  /// </summary>
  /// <param name="speed">the speed at which to shake (higher means faster side to side)</param>
  /// <param name="strength">the strength at which to shake (higher means shaking moves the camera further</param>
  /// <param name="duration">the duration of time in seconds to perform this screen shake</param>
  public static void Shake(float speed, float strength, float duration) {
    var str = strength * ScreenShakeStrength;
    var dur = Mathf.Clamp(duration, 0.0f, MaxScreenShakeDuration);
    RequestScreenShake?.Invoke(speed, str, 1.0f / dur);
  }

  /// <summary>
  ///Triggers a controller rumble (if supported on hardware)
  /// </summary>
  /// <param name="strong">intensity for the "strong" rumble motor(s) (varies by hardware, XBox & PS tend to be near end of controller nubs)</param>
  /// <param name="weak">intensity for the "weak" rumble motor(s) (varies by hardware. XBox & PS tend towards the main 'brick' segment of the controller)</param>
  /// <param name="duration">the duration of time in seconds for this rumble to last</param>
  /// <param name="controller_id">optionally a specific controller id to rumble. Useful for counch multiplayer. If left default (-1), all connected controllers are rumbled. </param>
  public static void Rumble(float strong, float weak, float duration = 0.1f, int controller_id = -1) {
    var current = Input.GetConnectedJoypads();
    var len = duration * MaxRumbleDuration;
    var str = Mathf.Clamp(strong * RumbleStrength, 0.0f, 1.0f);
    var wee = Mathf.Clamp(weak * RumbleStrength, 0.0f, 1.0f);

    if (controller_id < 0) {
      foreach (var index in current) {
        Input.StartJoyVibration(index, str, wee, len);
      }
    }
    else {
      Input.StartJoyVibration(controller_id, str, wee, len);
    }
  }

  private const string FILE_PATH = "effects.json";

  public static void Load() {
    EventBus.Data.SerializeAll += SaveSettings;
    var builder = new SaveDataBuilder(FILE_PATH, useCurrentSaveSlot: false).LoadFromFile();
    RumbleStrength = builder.GetFloat(nameof(RumbleStrength), out var f1) ? f1 : RumbleStrength;
    MaxRumbleDuration = builder.GetFloat(nameof(MaxRumbleDuration), out var f2) ? f2 : MaxRumbleDuration;
    ScreenShakeStrength = builder.GetFloat(nameof(ScreenShakeStrength), out var f3) ? f3 : ScreenShakeStrength;
    MaxScreenShakeDuration = builder.GetFloat(nameof(MaxScreenShakeDuration), out var f4) ? f4 : MaxScreenShakeDuration;
  }

  public static void SaveSettings() {
    var builder = new SaveDataBuilder(FILE_PATH, useCurrentSaveSlot: false);
    builder.PutFloat(nameof(RumbleStrength), RumbleStrength);
    builder.PutFloat(nameof(MaxRumbleDuration), MaxRumbleDuration);
    builder.PutFloat(nameof(ScreenShakeStrength), ScreenShakeStrength);
    builder.PutFloat(nameof(MaxScreenShakeDuration), MaxScreenShakeDuration);
    builder.SaveToFile();
  }
}
