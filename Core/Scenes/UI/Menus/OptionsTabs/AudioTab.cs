using System;
using Godot;
using queen.data;
using queen.events;
using queen.extension;

public partial class AudioTab : PanelContainer
{
    [Export] private NodePath path_slider_main;
    [Export] private NodePath path_slider_vo;
    [Export] private NodePath path_slider_sfx;
    [Export] private NodePath path_slider_creature;
    [Export] private NodePath path_slider_drones;

    private HSlider slider_main;
    private HSlider slider_vo;
    private HSlider slider_sfx;
    private HSlider slider_creature;
    private HSlider slider_drones;

    public override void _Ready()
    {
        this.GetNode(path_slider_main, out slider_main);
        this.GetNode(path_slider_vo, out slider_vo);
        this.GetNode(path_slider_sfx, out slider_sfx);
        this.GetNode(path_slider_creature, out slider_creature);
        this.GetNode(path_slider_drones, out slider_drones);

        slider_main.Value = AudioBuses.Instance.VolumeMain;
        slider_vo.Value = AudioBuses.Instance.VolumeVO;
        slider_sfx.Value = AudioBuses.Instance.VolumeSFX;
        slider_creature.Value = AudioBuses.Instance.VolumeCreature;
        slider_drones.Value = AudioBuses.Instance.VolumeDrones;

        Events.Data.SerializeAll += ApplyChanges;
    }

    public override void _ExitTree()
    {
        Events.Data.SerializeAll -= ApplyChanges;
    }



    public void ApplyChanges()
    {
        AudioBuses.Instance.VolumeMain = (float)slider_main.Value;
        AudioBuses.Instance.VolumeVO = (float)slider_vo.Value;
        AudioBuses.Instance.VolumeSFX = (float)slider_sfx.Value;
        AudioBuses.Instance.VolumeCreature = (float)slider_creature.Value;
        AudioBuses.Instance.VolumeDrones = (float)slider_drones.Value;

        AudioBuses.SaveSettings();
    }
}
