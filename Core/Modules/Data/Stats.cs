namespace Squiggles.Core.Data;

using Squiggles.Core.Events;

/// <summary>
/// A rudimentary attempt at tracking some basic statistics about the user. Nothing "phones home" or anything. Just to provide the user meaningful data on their session time. Unfortunately, I never really finished making  this part, and I'm unsure if its a feature with any demand. Most launchers manage runtime anyway so this could be completely unnecessary
/// </summary>
public class Stats {

  //
  //  Meaningful information
  //      Defaults assigned as well
  //
  public bool FirstTimeLaunch = true;
  public float TotalPlayTime;




  //
  //  Singleton Setup
  //
  public static Stats Instance {
    get {
      if (_instance is null) {
        CreateInstance();
      }

      return _instance;

    }
  }
  private static Stats _instance;
  private const string FILE_PATH = "stats.json";

  public static void ForceLoadInstance() {
    if (_instance != null) {
      return;
    }

    CreateInstance();
  }

  private static void CreateInstance() {
    _instance = new Stats();
    var loaded = SaveData.Load<Stats>(FILE_PATH);
    if (loaded != null) {
      _instance = loaded;
    }
    EventBus.Data.SerializeAll += SaveSettings;

  }

  public static void SaveSettings() {
    if (_instance == null) {
      return;
    }

    SaveData.Save(_instance, FILE_PATH);
  }

}
