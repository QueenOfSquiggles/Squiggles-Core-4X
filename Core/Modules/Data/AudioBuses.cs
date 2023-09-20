namespace Squiggles.Core.Data;

using System;
using Godot;
using Squiggles.Core.Events;

public class AudioBuses {
  public float[] Volumes { get; set; } = Array.Empty<float>();

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
