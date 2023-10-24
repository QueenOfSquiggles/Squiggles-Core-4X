namespace Squiggles.Core.Data;

using Squiggles.Core.Events;

/// <summary>
/// A rudimentary attempt at tracking some basic statistics about the user. Nothing "phones home" or anything. Just to provide the user meaningful data on their session time. Unfortunately, I never really finished making  this part, and I'm unsure if its a feature with any demand. Most launchers manage runtime anyway so this could be completely unnecessary
/// </summary>
public static class Stats {

  //
  //  Meaningful information
  //      Defaults assigned as well
  //
  public static bool FirstTimeLaunch { get; set; } = true;
  public static float TotalPlayTime { get; set; }




  private const string FILE_PATH = "stats.json";

  public static void Load() {
    var builder = new SaveDataBuilder(FILE_PATH, useCurrentSaveSlot: false).LoadFromFile();
    FirstTimeLaunch = builder.GetBool(nameof(FirstTimeLaunch), out var b1) ? b1 : FirstTimeLaunch;
    TotalPlayTime = builder.GetFloat(nameof(TotalPlayTime), out var f1) ? f1 : TotalPlayTime;

    EventBus.Data.SerializeAll += SaveSettings;
  }

  public static void SaveSettings() {
    var builder = new SaveDataBuilder(FILE_PATH, useCurrentSaveSlot: false);
    builder.PutBool(nameof(FirstTimeLaunch), FirstTimeLaunch); // intentionally hard-coded to ensure reasonable detection of first launch
    builder.PutFloat(nameof(TotalPlayTime), TotalPlayTime);
    builder.SaveToFile();
  }

}
