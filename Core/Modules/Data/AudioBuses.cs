namespace Squiggles.Core.Data;

using System;
using Godot;
using Squiggles.Core.Events;

/// <summary>
/// The singleton for managing the Godot Audio Bus with a decent layer of abstraction.
/// </summary>
public static class AudioBuses {
  /// <summary>
  /// The volumes (dB) for each audio bus track currently loaded
  /// </summary>
  public static float[] Volumes { get; set; } = Array.Empty<float>();

  private const string FILE_PATH = "audio.json";

  public static void Load() {
    EventBus.Data.SerializeAll += SaveSettings;
    UpdateAudioServer();
    var builder = new SaveDataBuilder(FILE_PATH, useCurrentSaveSlot: false).LoadFromFile();
    foreach (var entry in builder.Iterator) {
      if (!entry.Key.StartsWith("bus")) { continue; }
      if (int.TryParse(entry.Key.AsSpan(3), out var busIndex) && busIndex >= 0 && busIndex < Volumes.Length) {
        Volumes[busIndex] = builder.GetFloat(entry.Key, out var value) ? value : Volumes[busIndex];
      }
    }
  }

  /// <summary>
  /// Saves settings of this singleton out to disk (global scope)
  /// </summary>
  public static void SaveSettings() {
    UpdateAudioServer();
    var builder = new SaveDataBuilder(FILE_PATH, useCurrentSaveSlot: false);
    for (var i = 0; i < Volumes.Length; i++) {
      builder.PutFloat($"bus{i}", Volumes[i]);
    }
    builder.SaveToFile();
  }

  private static void UpdateAudioServer() {
    if (Volumes.Length != AudioServer.BusCount) {
      var temp = new float[AudioServer.BusCount];
      for (var i = 0; i < Volumes.Length; i++) {
        if (i >= temp.Length) {
          break;
        }

        temp[i] = Volumes[i];
      }
      Volumes = temp;
    }

    for (var i = 0; i < AudioServer.BusCount; i++) {
      AudioServer.SetBusVolumeDb(i, Volumes[i]);
    }
  }

}
