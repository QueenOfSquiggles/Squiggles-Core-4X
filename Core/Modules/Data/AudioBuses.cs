namespace Squiggles.Core.Data;

using System;
using Godot;
using Squiggles.Core.Events;

/// <summary>
/// The singleton for managing the Godot Audio Bus with a decent layer of abstraction.
/// </summary>
public class AudioBuses {
  /// <summary>
  /// The volumes (dB) for each audio bus track currently loaded
  /// </summary>
  public float[] Volumes { get; set; } = Array.Empty<float>();

  /// <summary>
  /// Utility to quickly access and modify volumes from this singleton.
  /// </summary>
  /// <param name="index">the index of the desired audio bus</param>
  /// <returns>the current volume of the current audio bus</returns>
  public float this[int index] {
    get => Volumes[index];
    set => Volumes[index] = value;
  }

  //
  //  Singleton Setup
  //
  public static AudioBuses Instance {
    get {
      if (_instance is null) {
        CreateInstance();
      }

      return _instance;

    }
  }
  private static AudioBuses _instance;
  private const string FILE_PATH = "audio.json";

  private static void CreateInstance() {
    _instance = new AudioBuses();
    var loaded = SaveData.Load<AudioBuses>(FILE_PATH);
    if (loaded != null) {
      _instance = loaded;
    }
    EventBus.Data.SerializeAll += SaveSettings;

    UpdateAudioServer();
  }

  /// <summary>
  /// Saves settings of this singleton out to disk (global scope)
  /// </summary>
  public static void SaveSettings() {
    if (_instance == null) {
      return;
    }

    UpdateAudioServer();
    SaveData.Save(_instance, FILE_PATH);
  }

  private static void UpdateAudioServer() {
    if (_instance is null) {
      return;
    }

    if (_instance.Volumes.Length != AudioServer.BusCount) {
      var temp = new float[AudioServer.BusCount];
      for (var i = 0; i < _instance.Volumes.Length; i++) {
        if (i >= temp.Length) {
          break;
        }

        temp[i] = _instance.Volumes[i];
      }
      _instance.Volumes = temp;
    }

    for (var i = 0; i < AudioServer.BusCount; i++) {
      AudioServer.SetBusVolumeDb(i, _instance.Volumes[i]);
    }
  }

}
