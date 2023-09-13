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
    [Export] private OptionButton _OptionFullscreen;
    [Export] private CheckBox _CheckBloom;
    [Export] private CheckBox _CheckSSR;
    [Export] private CheckBox _CheckSSAO;
    [Export] private CheckBox _CheckSSIL;
    [Export] private CheckBox _CheckSDFGI;
    [Export] private HSlider _SliderExposure;
    [Export] private HSlider _SliderBrightness;
    [Export] private HSlider _SliderContrast;
    [Export] private HSlider _SliderSaturation;

    public override void _Ready()
    {
        if (_OptionFullscreen is null ||
            _CheckBloom is null ||
            _CheckSSR is null ||
            _CheckSSAO is null ||
            _CheckSSIL is null ||
            _CheckSDFGI is null ||
            _SliderExposure is null ||
            _SliderBrightness is null ||
            _SliderContrast is null ||
            _SliderSaturation is null
        ) return;

        int current = _OptionFullscreen.GetItemIndex(Graphics.Instance.Fullscreen);
        _OptionFullscreen.Selected = current;

        _CheckBloom.ButtonPressed = Graphics.Instance.Bloom;
        _CheckSSR.ButtonPressed = Graphics.Instance.SSR;
        _CheckSSAO.ButtonPressed = Graphics.Instance.SSAO;
        _CheckSSIL.ButtonPressed = Graphics.Instance.SSIL;
        _CheckSDFGI.ButtonPressed = Graphics.Instance.SDFGI;
        _SliderExposure.Value = Graphics.Instance.TonemapExposure;
        _SliderBrightness.Value = Graphics.Instance.Brightness;
        _SliderContrast.Value = Graphics.Instance.Contrast;
        _SliderSaturation.Value = Graphics.Instance.Saturation;

        Events.Data.SerializeAll += ApplyGraphicsSettings;

        var root = GetNode(PathGraphicsDisplayRoot) as Control;
        if (root is not null)
        {
            var scene = PackedGraphicsDisplay?.Instantiate();
            if (scene is not null) root.AddChild(scene);
        }
    }

    public override void _ExitTree()
    {
        Events.Data.SerializeAll -= ApplyGraphicsSettings;
    }

    public void ApplyGraphicsSettings()
    {
        if (_OptionFullscreen is null ||
            _CheckBloom is null ||
            _CheckSSR is null ||
            _CheckSSAO is null ||
            _CheckSSIL is null ||
            _CheckSDFGI is null ||
            _SliderExposure is null ||
            _SliderBrightness is null ||
            _SliderContrast is null ||
            _SliderSaturation is null
        ) return;

        Graphics.Instance.Fullscreen = _OptionFullscreen.GetSelectedId();
        Graphics.Instance.Bloom = _CheckBloom.ButtonPressed;
        Graphics.Instance.SSR = _CheckSSR.ButtonPressed;
        Graphics.Instance.SSAO = _CheckSSAO.ButtonPressed;
        Graphics.Instance.SSIL = _CheckSSIL.ButtonPressed;
        Graphics.Instance.SDFGI = _CheckSDFGI.ButtonPressed;
        Graphics.Instance.TonemapExposure = (float)_SliderExposure.Value;
        Graphics.Instance.Brightness = (float)_SliderBrightness.Value;
        Graphics.Instance.Contrast = (float)_SliderContrast.Value;
        Graphics.Instance.Saturation = (float)_SliderSaturation.Value;
        Graphics.MarkGraphicsChanged();
    }
}
