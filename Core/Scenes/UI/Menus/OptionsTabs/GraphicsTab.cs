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

  [Export] private Control[] _colourCorrectionControls;

  private bool _useColourCorrection = true;

  public override void _Ready() {
    _useColourCorrection = ThisIsYourMainScene.Config?.EnableColourCorrection is true;

    var current = _optionFullscreen.GetItemIndex(Graphics.Instance.Fullscreen);
    _optionFullscreen.Selected = current;

    _checkBloom.ButtonPressed = Graphics.Instance.Bloom;
    _checkSSR.ButtonPressed = Graphics.Instance.SSR;
    _checkSSAO.ButtonPressed = Graphics.Instance.SSAO;
    _checkSSIL.ButtonPressed = Graphics.Instance.SSIL;
    _checkSDFGI.ButtonPressed = Graphics.Instance.SDFGI;

    if (_useColourCorrection) {
      _sliderExposure.Value = Graphics.Instance.TonemapExposure;
      _sliderBrightness.Value = Graphics.Instance.Brightness;
      _sliderContrast.Value = Graphics.Instance.Contrast;
      _sliderSaturation.Value = Graphics.Instance.Saturation;
    }
    else {
      foreach (var c in _colourCorrectionControls) {
        c.QueueFree();
      }
    }

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
    if (_useColourCorrection) {
      Graphics.Instance.TonemapExposure = (float)_sliderExposure.Value;
      Graphics.Instance.Brightness = (float)_sliderBrightness.Value;
      Graphics.Instance.Contrast = (float)_sliderContrast.Value;
      Graphics.Instance.Saturation = (float)_sliderSaturation.Value;
    }
    Graphics.MarkGraphicsChanged();
  }
}
