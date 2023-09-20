namespace Squiggles.Core.Scenes.UI.Menus;

using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Events;


public partial class GraphicsTab : PanelContainer {
  [Export] private PackedScene _packedGraphicsDisplay;

  [ExportGroup("Node Paths")]
  [Export] private NodePath _pathGraphicsDisplayRoot = "MarginContainer/VBoxContainer/GraphicsDisplayRoot";
  [Export] private OptionButton _optionFullscreen;
  [Export] private CheckBox _checkBloom;
  [Export] private CheckBox _checkSSR;
  [Export] private CheckBox _checkSSAO;
  [Export] private CheckBox _checkSSIL;
  [Export] private CheckBox _checkSDFGI;
  [Export] private HSlider _sliderExposure;
  [Export] private HSlider _sliderBrightness;
  [Export] private HSlider _sliderContrast;
  [Export] private HSlider _sliderSaturation;

  public override void _Ready() {
    if (_optionFullscreen is null ||
        _checkBloom is null ||
        _checkSSR is null ||
        _checkSSAO is null ||
        _checkSSIL is null ||
        _checkSDFGI is null ||
        _sliderExposure is null ||
        _sliderBrightness is null ||
        _sliderContrast is null ||
        _sliderSaturation is null
    ) {
      return;
    }

    var current = _optionFullscreen.GetItemIndex(Graphics.Instance.Fullscreen);
    _optionFullscreen.Selected = current;

    _checkBloom.ButtonPressed = Graphics.Instance.Bloom;
    _checkSSR.ButtonPressed = Graphics.Instance.SSR;
    _checkSSAO.ButtonPressed = Graphics.Instance.SSAO;
    _checkSSIL.ButtonPressed = Graphics.Instance.SSIL;
    _checkSDFGI.ButtonPressed = Graphics.Instance.SDFGI;
    _sliderExposure.Value = Graphics.Instance.TonemapExposure;
    _sliderBrightness.Value = Graphics.Instance.Brightness;
    _sliderContrast.Value = Graphics.Instance.Contrast;
    _sliderSaturation.Value = Graphics.Instance.Saturation;

    EventBus.Data.SerializeAll += ApplyGraphicsSettings;

    var root = GetNode(_pathGraphicsDisplayRoot) as Control;
    if (root is not null) {
      var scene = _packedGraphicsDisplay?.Instantiate();
      if (scene is not null) {
        root.AddChild(scene);
      }
    }
  }

  public override void _ExitTree() => EventBus.Data.SerializeAll -= ApplyGraphicsSettings;

  public void ApplyGraphicsSettings() {
    Graphics.Instance.Fullscreen = _optionFullscreen.GetSelectedId();
    Graphics.Instance.Bloom = _checkBloom.ButtonPressed;
    Graphics.Instance.SSR = _checkSSR.ButtonPressed;
    Graphics.Instance.SSAO = _checkSSAO.ButtonPressed;
    Graphics.Instance.SSIL = _checkSSIL.ButtonPressed;
    Graphics.Instance.SDFGI = _checkSDFGI.ButtonPressed;
    Graphics.Instance.TonemapExposure = (float)_sliderExposure.Value;
    Graphics.Instance.Brightness = (float)_sliderBrightness.Value;
    Graphics.Instance.Contrast = (float)_sliderContrast.Value;
    Graphics.Instance.Saturation = (float)_sliderSaturation.Value;
    Graphics.MarkGraphicsChanged();
  }
}
