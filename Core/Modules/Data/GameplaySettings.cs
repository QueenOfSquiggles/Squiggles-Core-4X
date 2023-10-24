namespace Squiggles.Core.Data;

using System;
using System.Collections.Generic;
using Squiggles.Core.Events;

/// <summary>
/// The singleton for handling gameplay settings. These are designed to be highly configurable with a provider approach.
/// </summary>
public static class GameplaySettings {

  //
  //  Meaningful information
  //      Defaults assigned as well
  //
  /// <summary>
  /// The loaded options with a key-value pairing where both are strings. They are then parsed when requested.
  /// </summary>
  public static readonly Dictionary<string, string> Options = new();

  //
  //  Helper Functions
  //

  // events
  /// <summary>
  /// An event that triggers whenever the options have changed. Listen to this to allow response to changes in the settings (which can be done from the pause menu!!!!)
  /// </summary>
  public static event Action OptionsChanged;


  /// <summary>
  /// Determines whether a key is present in the settings. Generally as long as you did everything right, this should eval true.
  /// </summary>
  /// <param name="key">the key to check for</param>
  /// <returns>true if the key is present. False if not</returns>
  public static bool HasKey(string key) => Options.ContainsKey(key);

  // Getters
  /// <summary>
  /// Gets the option cast to a boolssss
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  public static bool GetBool(string key) {
    if (Options.ContainsKey(key)) {
      var success = bool.TryParse(Options[key], out var result);
      return success && result;
    }
    return false;
  }

  /// <summary>
  /// Gets the option cast to a float
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  public static float GetFloat(string key) {
    if (Options.ContainsKey(key)) {
      var success = float.TryParse(Options[key], out var result);
      return success ? result : 0.0f;
    }
    return 0.0f;
  }

  /// <summary>
  /// Gets the option as a string (which is how it is stored so no casting occurs)
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  public static string GetString(string key) => Options.ContainsKey(key) ? Options[key] : "";

  // Setters
  /// <summary>
  /// Sets an option with the given bool value
  /// </summary>
  /// <param name="key"></param>
  /// <param name="value"></param>
  public static void SetBool(string key, bool value) {
    Options[key] = value.ToString();
    OptionsChanged?.Invoke();
  }

  /// <summary>
  /// Sets an option with the given float value
  /// </summary>
  /// <param name="key"></param>
  /// <param name="value"></param>
  public static void SetFloat(string key, float value) {
    Options[key] = value.ToString();
    OptionsChanged?.Invoke();
  }

  /// <summary>
  /// Sets an option with the given string value
  /// </summary>
  /// <param name="key"></param>
  /// <param name="value"></param>
  public static void SetString(string key, string value) {
    Options[key] = value;
    OptionsChanged?.Invoke();
  }

  private const string FILE_PATH = "gameplay.json";

  public static void Load() {
    var builder = new SaveDataBuilder(FILE_PATH, useCurrentSaveSlot: false).LoadFromFile();
    foreach (var entry in builder.Iterator) {
      Options[entry.Key] = entry.Value;
    }
    OptionsChanged?.Invoke();
    EventBus.Data.SerializeAll += SaveSettings;
  }

  public static void SaveSettings() {
    var builder = new SaveDataBuilder(FILE_PATH, useCurrentSaveSlot: false);
    foreach (var entry in Options) {
      builder.PutString(entry.Key, entry.Value);
    }
    builder.SaveToFile();
  }
}
