namespace Squiggles.Core.Data;

using System.Collections.Generic;
using Squiggles.Core.Events;

public class GameplaySettings {

  //
  //  Meaningful information
  //      Defaults assigned as well
  //
  public Dictionary<string, string> Options = new();

  //
  //  Helper Functions
  //


  public static bool HasKey(string key) => Instance.Options.ContainsKey(key);

  // Getters
  public static bool GetBool(string key) {
    if (Instance.Options.ContainsKey(key)) {
      var success = bool.TryParse(Instance.Options[key], out var result);
      return success || result;
    }
    return false;
  }

  public static float GetFloat(string key) {
    if (Instance.Options.ContainsKey(key)) {
      var success = float.TryParse(Instance.Options[key], out var result);
      return success ? result : 0.0f;
    }
    return 0.0f;
  }

  public static string GetString(string key) => Instance.Options.ContainsKey(key) ? Instance.Options[key] : "";

  // Setters
  public static void SetBool(string key, bool value) => Instance.Options[key] = value.ToString();
  public static void SetFloat(string key, float value) => Instance.Options[key] = value.ToString();
  public static void SetString(string key, string value) => Instance.Options[key] = value;

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
