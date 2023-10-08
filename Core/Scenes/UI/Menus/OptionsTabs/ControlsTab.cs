namespace Squiggles.Core.Scenes.UI.Menus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Error;
using Squiggles.Core.Events;

/// <summary>
/// The tab which handles controls sensitivity and rebindings. Refer to <see cref="Controls"/> for how these are consumed. For details on how to set up which mappings are displayed, refer to <see cref="SquigglesCoreConfigFile.RemapControlsNames"/>
/// </summary>
public partial class ControlsTab : PanelContainer {

  /// <summary>
  /// Whether or not this tab is actively listening for inputs to bind to
  /// </summary>
  private bool _listening;
  /// <summary>
  /// The current actionm mapping key to listen for
  /// </summary>
  private string _currentActionTarget = "";
  /// <summary>
  /// The popup used to signal that we are listening for input
  /// </summary>
  [Export] private Popup _popupListening;
  /// <summary>
  /// The slider for mouse look sensitivity
  /// </summary>
  [Export] private Slider _sliderMouse;
  /// <summary>
  /// The slider for gamepad look sensitivity
  /// </summary>
  [Export] private Slider _sliderGamepad;

  /// <summary>
  /// The root of the mappings section
  /// </summary>
  [ExportGroup("Mappings", "_Mapping")]
  [Export] private Control _mappingRoot;
  /// <summary>
  /// The scene which is loaded for each mapping.
  /// </summary>
  [Export] private PackedScene _mappingScene;

  public override void _Ready() {
    _popupListening.Exclusive = true;
    _popupListening.WindowInput += _Input;

    _sliderMouse.Value = Controls.MouseLookSensivity;
    _sliderGamepad.Value = Controls.ControllerLookSensitivity;

    EventBus.Data.SerializeAll += ApplyChanges;

    var keys = SC4X.Config?.RemapControlsNames ?? Array.Empty<string>();
    if (keys.Length <= 0) {
      // empty array, assume all are valid and place custom mappings first
      var mappings = InputMap.GetActions();
      var union = new List<StringName>();
      var custom_mappings = mappings.Where((key) => !key.ToString().StartsWith("ui")).ToList();
      union.AddRange(custom_mappings);
      if (!(SC4X.Config?.HideUIMappings ?? true)) {
        var ui_mappings = mappings.Where((key) => key.ToString().StartsWith("ui")).ToList();
        union.AddRange(ui_mappings);
      }

      keys = new string[union.Count];
      for (var i = 0; i < union.Count; i++) {
        keys[i] = union[i];
      }
    }
    foreach (var action in keys) {
      if (!InputMap.HasAction(action)) { // allow for input mappings to filter out unsupported controls
        continue;
      }
      if (_mappingScene?.Instantiate() is not ActionMappingSlot scene) {
        continue;
      }

      scene.Name = $"Remam_{action}";
      scene.TargetAction = action;
      _mappingRoot?.AddChild(scene);
      scene.ListenForAction += ListenForAction;
    }
  }

  public override void _ExitTree() => EventBus.Data.SerializeAll -= ApplyChanges;

  public async void ListenForAction(string action_name) {
    _currentActionTarget = action_name;
    _popupListening?.PopupCenteredRatio();
    await Task.Delay(50);
    _listening = true;
  }

  public override void _Input(InputEvent @event) {
    if (!_listening || _currentActionTarget.Length == 0) {
      return;
    }

    var is_valid = false;

    switch (@event) {
      case InputEventKey key:
        is_valid = key.Pressed;
        break;
      case InputEventJoypadButton joy:
        is_valid = joy.Pressed;
        break;
      case InputEventMouseButton mou:
        is_valid = mou.Pressed;
        break;
      case InputEventJoypadMotion axis:
        is_valid = Mathf.Abs(axis.AxisValue) > 0.999f; // force max to avoid noise/drift
        break;
      default:
        break;
    }

    if (is_valid) {
      Print.Debug($"Processing input event override for action {_currentActionTarget}, received event: {@event.AsText()}");
      Controls.SetMapping(_currentActionTarget, @event);
      _currentActionTarget = "";
      _listening = false;
      _popupListening?.Hide();
    }
  }

  public void ResetAllMappings() => Controls.ResetMappings();

  public void ApplyChanges() {
    Controls.MouseLookSensivity = (float)(_sliderMouse?.Value ?? 0);
    Controls.ControllerLookSensitivity = (float)(_sliderGamepad?.Value ?? 0);
    Controls.SaveSettings();
  }

}
