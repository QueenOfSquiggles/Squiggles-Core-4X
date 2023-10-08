namespace Squiggles.Core.Data;

using System;
using System.Collections.Generic;
using Squiggles.Core.Events;
using Squiggles.Core.Scenes;
using Squiggles.Core.Scenes.UI.Menus.Gameplay;

/// <summary>
/// The singleton for handling gameplay settings. These are designed to be highly configurable with a provider approach.
/// </summary>
public class GameplaySettings {

  //
  //  Meaningful information
  //      Defaults assigned as well
  //
  /// <summary>
  /// The loaded options with a key-value pairing where both are strings. They are then parsed when requested.
  /// </summary>
  public readonly Dictionary<string, string> Options = new();

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
  public static bool HasKey(string key) => Instance.Options.ContainsKey(key);

  // Getters
  /// <summary>
  /// Gets the option cast to a boolssss
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  public static bool GetBool(string key) {
    if (Instance.Options.ContainsKey(key)) {
      var success = bool.TryParse(Instance.Options[key], out var result);
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
    if (Instance.Options.ContainsKey(key)) {
      var success = float.TryParse(Instance.Options[key], out var result);
      return success ? result : 0.0f;
    }
    return 0.0f;
  }

  /// <summary>
  /// Gets the option as a string (which is how it is stored so no casting occurs)
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  public static string GetString(string key) => Instance.Options.ContainsKey(key) ? Instance.Options[key] : "";

  // Setters
  /// <summary>
  /// Sets an option with the given bool value
  /// </summary>
  /// <param name="key"></param>
  /// <param name="value"></param>
  public static void SetBool(string key, bool value) {
    Instance.Options[key] = value.ToString();
    OptionsChanged?.Invoke();
  }

  /// <summary>
  /// Sets an option with the given float value
  /// </summary>
  /// <param name="key"></param>
  /// <param name="value"></param>
  public static void SetFloat(string key, float value) {
    Instance.Options[key] = value.ToString();
    OptionsChanged?.Invoke();
  }

  /// <summary>
  /// Sets an option with the given string value
  /// </summary>
  /// <param name="key"></param>
  /// <param name="value"></param>
  public static void SetString(string key, string value) {
    Instance.Options[key] = value;
    OptionsChanged?.Invoke();
  }

  //
  //  Singleton Setup
  //
  public static GameplaySettings Instance {
    get {
      if (_instance is null) {
        CreateInstance();
      }

      return _instance;
    }
  }
  private static GameplaySettings _instance;
  private const string FILE_PATH = "gameplay.json";

  private static void CreateInstance() {
    _instance = new GameplaySettings();
    var loaded = SaveData.Load<GameplaySettings>(FILE_PATH);
    if (loaded != null) {
      _instance = loaded;
    }
    else {
      // load from config file
      if (SC4X.Config?.GameplayConfig?.OptionsArray is OptionBase[] options) {
        foreach (var op in options) {
          switch (op) {
            case OptionBool opb:
              SetBool(opb.InternalName, opb.Value);
              break;
            case OptionSlider ops:
              SetFloat(ops.InternalName, ops.DefaultValue);
              break;
            case OptionComboSelect opcs:
              SetString(opcs.InternalName, opcs.Options[opcs.DefaultSelection]);
              break;
          }
        }
      }
    }
    // Print.Info("Gameplay Options Loaded:");
    // foreach (var pair in _instance.Options) {
    //   Print.Info($"\t\"{pair.Key}\" = '{pair.Value}'");
    // }
    EventBus.Data.SerializeAll += SaveSettings;
  }

  public static void SaveSettings() {
    if (_instance == null) {
      return;
    }

    SaveData.Save(_instance, FILE_PATH);
  }
}
