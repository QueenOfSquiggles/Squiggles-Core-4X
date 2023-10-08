namespace Squiggles.Core.Scenes.UI.Menus;

using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Events;

/// <summary>
/// The tab for controlling audio volumes. It dynamically adjusts to whatever audio bus layout you have established.
/// </summary>
public partial class AudioTab : PanelContainer {
  [Export] private Control _slidersRoot;
  [Export] private PackedScene _sliderComboScene;

  public override void _Ready() {
    var count = AudioServer.BusCount;
    for (var i = 0; i < count; i++) {
      var scene = _sliderComboScene?.Instantiate();
      if (scene is null) {
        continue;
      }

      var bus_name = AudioServer.GetBusName(i);
      scene.Name = $"AudioSlider_{bus_name}";
      var sci = new SliderComboAdapter(scene) {
        Text = bus_name,
        MinValue = -72,
        MaxValue = -6,
        StepValue = 1,
        SliderValue = -15
      };
      _slidersRoot?.AddChild(scene);
    }


    EventBus.Data.SerializeAll += ApplyChanges;
  }

  public override void _ExitTree() => EventBus.Data.SerializeAll -= ApplyChanges;


  public void ApplyChanges() {
    var count = AudioBuses.Volumes.Length;
    AudioBuses.Load(); // force load, which will resize volumes array
    for (var i = 0; i < count; i++) {
      var slider = _slidersRoot?.GetChild(i);
      if (slider is null) {
        continue;
      }

      var sci = new SliderComboAdapter(slider);
      AudioBuses.Volumes[i] = sci.SliderValue;
    }
    AudioBuses.SaveSettings();
  }
}
