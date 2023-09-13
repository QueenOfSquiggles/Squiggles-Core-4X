using System;
using Godot;
using queen.data;
using queen.events;
using queen.extension;

public partial class AudioTab : PanelContainer
{
    [Export] private Control _SlidersRoot;
    [Export] private PackedScene _SliderComboScene;

    public override void _Ready()
    {
        int count = AudioServer.BusCount;
        for (int i = 0; i < count; i++)
        {
            var scene = _SliderComboScene?.Instantiate();
            if (scene is null) continue;
            var bus_name = AudioServer.GetBusName(i);
            scene.Name = $"AudioSlider_{bus_name}";
            var sci = new SliderComboInterface(scene)
            {
                Text = bus_name,
                MinValue = -72,
                MaxValue = -6,
                StepValue = 1,
                SliderValue = -15
            };
            _SlidersRoot?.AddChild(scene);
        }


        Events.Data.SerializeAll += ApplyChanges;
    }

    public override void _ExitTree()
    {
        Events.Data.SerializeAll -= ApplyChanges;
    }


    public void ApplyChanges()
    {
        int count = AudioBuses.Instance.Volumes.Length;
        var _ = AudioBuses.Instance; // force load, which will resize volumes array
        for (int i = 0; i < count; i++)
        {
            var slider = _SlidersRoot?.GetChild(i);
            if (slider is null) continue;
            var sci = new SliderComboInterface(slider);
            AudioBuses.Instance.Volumes[i] = sci.SliderValue;
        }
        AudioBuses.SaveSettings();
    }
}
