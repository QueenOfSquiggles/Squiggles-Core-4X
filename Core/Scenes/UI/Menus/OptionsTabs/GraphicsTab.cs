namespace Squiggles.Core.Scenes.UI.Menus;

using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Events;

/// <summary>
/// The tab that handles the graphics settings for this game. Refer to <see cref="Graphics"/> and <see cref="WorldEnvSettingsCompliant"/> for details on how these are used.
/// </summary>
public partial class GraphicsTab : PanelContainer {
  /// <summary>
  /// A packed scene that is used for the preview of graphics settings
  /// </summary>
  [Export] private PackedScene _packedGraphicsDisplay;

  /// <summary>
  /// The root of the display
  /// </summary>
  [ExportGroup("Node Paths")]
  [Export] private NodePath _pathGraphicsDisplayRoot = "MarginContainer/VBoxContainer/GraphicsDisplayRoot";
  /// <summary>
  /// The selection for what kind of windowing should be used.
  /// </summary>
  [Export] private OptionButton _optionFullscreen;
  /// <summary>
  /// Checkbox for whether or not to render bloom
  /// </summary>
  [Export] private CheckBox _checkBloom;
  /// <summary>
  /// Checkbox for whether or not to render SSR
  /// </summary>
  [Export] private CheckBox _checkSSR;
  /// <summary>
  /// Checkbox for whether or not to render SSAO
  /// </summary>
  [Export] private CheckBox _checkSSAO;
  /// <summary>
  /// Checkbox for whether or not to render SSIL
  /// </summary>
  [Export] private CheckBox _checkSSIL;
  /// <summary>
  /// Checkbox for whether or not to render SDFGI
  /// </summary>
  [Export] private CheckBox _checkSDFGI;
  /// <summary>
  /// Slider to determine exposure adjustment
  /// </summary>
  [Export] private HSlider _sliderExposure;
  /// <summary>
  /// Slider to determine exposure adjustment
  /// </summary>
  [Export] private HSlider _sliderBrightness;
  /// <summary>
  /// Slider to determine exposure adjustment
  /// </summary>
  [Export] private HSlider _sliderContrast;
  /// <summary>
  /// Slider to determine exposure adjustment
  /// </summary>
  [Export] private HSlider _sliderSaturation;

  /// <summary>
  /// An array of controls related to colour correction. Used to remove them all when configurations specify colour correction is disabled. Refer to <see cref="SquigglesCoreConfigFile.EnableColourCorrection"/>
  /// </summary>
  [Export] private Control[] _colourCorrectionControls;

  private bool _useColourCorrection = true;

  public override void _Ready() {
    _useColourCorrection = SC4X.Config?.EnableColourCorrection is true;

    var current = _optionFullscreen.GetItemIndex(Graphics.Fullscreen);
    _optionFullscreen.Selected = current;

    _checkBloom.ButtonPressed = Graphics.Bloom;
    _checkSSR.ButtonPressed = Graphics.SSR;
    _checkSSAO.ButtonPressed = Graphics.SSAO;
    _checkSSIL.ButtonPressed = Graphics.SSIL;
    _checkSDFGI.ButtonPressed = Graphics.SDFGI;

    if (_useColourCorrection) {
      _sliderExposure.Value = Graphics.TonemapExposure;
      _sliderBrightness.Value = Graphics.Brightness;
      _sliderContrast.Value = Graphics.Contrast;
      _sliderSaturation.Value = Graphics.Saturation;
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
    Graphics.Fullscreen = _optionFullscreen.GetSelectedId();
    Graphics.Bloom = _checkBloom.ButtonPressed;
    Graphics.SSR = _checkSSR.ButtonPressed;
    Graphics.SSAO = _checkSSAO.ButtonPressed;
    Graphics.SSIL = _checkSSIL.ButtonPressed;
    Graphics.SDFGI = _checkSDFGI.ButtonPressed;
    if (_useColourCorrection) {
      Graphics.TonemapExposure = (float)_sliderExposure.Value;
      Graphics.Brightness = (float)_sliderBrightness.Value;
      Graphics.Contrast = (float)_sliderContrast.Value;
      Graphics.Saturation = (float)_sliderSaturation.Value;
    }
    Graphics.MarkGraphicsChanged();
  }
}
