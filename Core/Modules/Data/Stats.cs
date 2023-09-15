namespace Squiggles.Core.Data;


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
  }

  public static void SaveSettings() {
    if (_instance == null) {
      return;
    }

    SaveData.Save(_instance, FILE_PATH);
  }

}
