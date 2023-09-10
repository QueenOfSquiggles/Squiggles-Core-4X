using System;
using Godot;
using queen.data;
using queen.events;
using queen.extension;

public partial class GraphicsTab : PanelContainer
{
    [Export] private PackedScene PackedGraphicsDisplay;

    [ExportGroup("Node Paths")]
    [Export] private NodePath PathGraphicsDisplayRoot = "MarginContainer/VBoxContainer/GraphicsDisplayRoot";
    [Export] private NodePath path_option_fullscreen;
    [Export] private NodePath path_check_bloom;
    [Export] private NodePath path_check_ssr;
    [Export] private NodePath path_check_ssao;
    [Export] private NodePath path_check_ssil;
    [Export] private NodePath path_check_sdfgi;
    [Export] private NodePath path_slider_exposure;
    [Export] private NodePath path_slider_brightness;
    [Export] private NodePath path_slider_contrast;
    [Export] private NodePath path_slider_saturation;
    private OptionButton option_fullscreen;
    private CheckBox check_bloom;
    private CheckBox check_ssr;
    private CheckBox check_ssao;
    private CheckBox check_ssil;
    private CheckBox check_sdfgi;
    private HSlider slider_exposure;
    private HSlider slider_brightness;
    private HSlider slider_contrast;
    private HSlider slider_saturation;

    public override void _Ready()
    {
        this.GetNode(path_option_fullscreen, out option_fullscreen);
        this.GetNode(path_check_bloom, out check_bloom);
        this.GetNode(path_check_ssr, out check_ssr);
        this.GetNode(path_check_ssao, out check_ssao);
        this.GetNode(path_check_ssil, out check_ssil);
        this.GetNode(path_check_sdfgi, out check_sdfgi);
        this.GetNode(path_slider_exposure, out slider_exposure);
        this.GetNode(path_slider_brightness, out slider_brightness);
        this.GetNode(path_slider_contrast, out slider_contrast);
        this.GetNode(path_slider_saturation, out slider_saturation);

        int current = option_fullscreen.GetItemIndex(Graphics.Instance.Fullscreen);
        option_fullscreen.Selected = current;

        check_bloom.ButtonPressed = Graphics.Instance.Bloom;
        check_ssr.ButtonPressed = Graphics.Instance.SSR;
        check_ssao.ButtonPressed = Graphics.Instance.SSAO;
        check_ssil.ButtonPressed = Graphics.Instance.SSIL;
        check_sdfgi.ButtonPressed = Graphics.Instance.SDFGI;
        slider_exposure.Value = Graphics.Instance.TonemapExposure;
        slider_brightness.Value = Graphics.Instance.Brightness;
        slider_contrast.Value = Graphics.Instance.Contrast;
        slider_saturation.Value = Graphics.Instance.Saturation;

        Events.Data.SerializeAll += ApplyGraphicsSettings;

        var root = GetNode(PathGraphicsDisplayRoot) as Control;
        if (root is not null)
        {
            var scene = PackedGraphicsDisplay.Instantiate();
            root.AddChild(scene);
        }
    }

    public override void _ExitTree()
    {
        Events.Data.SerializeAll -= ApplyGraphicsSettings;
    }

    public void ApplyGraphicsSettings()
    {
        Graphics.Instance.Fullscreen = option_fullscreen.GetSelectedId();
        Graphics.Instance.Bloom = check_bloom.ButtonPressed;
        Graphics.Instance.SSR = check_ssr.ButtonPressed;
        Graphics.Instance.SSAO = check_ssao.ButtonPressed;
        Graphics.Instance.SSIL = check_ssil.ButtonPressed;
        Graphics.Instance.SDFGI = check_sdfgi.ButtonPressed;
        Graphics.Instance.TonemapExposure = (float)slider_exposure.Value;
        Graphics.Instance.Brightness = (float)slider_brightness.Value;
        Graphics.Instance.Contrast = (float)slider_contrast.Value;
        Graphics.Instance.Saturation = (float)slider_saturation.Value;
        Graphics.MarkGraphicsChanged();
    }
}
