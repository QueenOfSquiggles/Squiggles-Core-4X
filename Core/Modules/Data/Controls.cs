namespace Squiggles.Core.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Squiggles.Core.Error;
using Squiggles.Core.Events;

/// <summary>
/// A singleton for managing controls mappings and sensitivities.
/// </summary>
public static class Controls {

  //
  //  Meaningful information
  //      Defaults assigned as well
  //

  /// <summary>
  /// Sensitivity to be applied when performing mouse look processes. Requires dev implementation
  /// </summary>
  public static float MouseLookSensivity { get; set; } = 300.0f;
  /// <summary>
  /// Sensitivity to be applied when performing gamepad/controller look processes. Requires dev implementation
  /// </summary>
  public static float ControllerLookSensitivity { get; set; } = 500.0f;

  /// <summary>
  /// The currently registerd overloads.
  /// </summary>
  public static Dictionary<string, int[]> MappingsOverloads { get; set; } = new();
  /// <summary>
  /// A cache of the original mappings to allow reverting easily.
  /// </summary>
  private static readonly Dictionary<string, List<InputEvent>> _originalMappingsCache = new();


  //
  //  Events
  //

  /// <summary>
  /// An event triggered when the mappings change
  /// </summary>
  public static event Action<string> OnControlMappingChanged;

  //
  //
  //
  private const int INPUT_KEY = 1;
  private const int INPUT_GAMEPAD_BUTTON = 2;
  private const int INPUT_MOUSE_BUTTON = 3;
  private const int INPUT_GAMEPAD_AXIS = 4;

  /// <summary>
  /// Resets all currently loaded mappings. Probably best you don't touch that buddy
  /// </summary>
  public static void ResetMappings() {
    var affected = new List<string>();
    foreach (var key in MappingsOverloads.Keys) {
      affected.Add(key);
    }
    InputMap.LoadFromProjectSettings();
    MappingsOverloads.Clear();
    foreach (var action in affected) {
      OnControlMappingChanged?.Invoke(action);
    }
  }

  /// <summary>
  /// Sets a provided event to a particular action as an override. Clears existing mappings before inserting new mapping
  /// </summary>
  /// <param name="action">the action key</param>
  /// <param name="assigned_input">the event to map to the action</param>
  public static void SetMapping(string action, InputEvent assigned_input) {
    if (!InputMap.HasAction(action)) {
      Print.Warn($"Failed to assign event {assigned_input.AsText()} to input action '{action}'. Mapping not found in InputMap");
      return;
    }

    if (!_originalMappingsCache.ContainsKey(action)) {
      // caches the originally loaded mapping
      var event_list = InputMap.ActionGetEvents(action);
      var list = new List<InputEvent>();
      list.AddRange(event_list.AsEnumerable());
      _originalMappingsCache.Add(action, list);
    }

    InputMap.ActionEraseEvents(action);
    InputMap.ActionAddEvent(action, assigned_input);

    MappingsOverloads[action] = GetInputCode(assigned_input);
    OnControlMappingChanged?.Invoke(action);
  }

  /// <summary>
  /// Resets a single mapping. Used internally. No touchey!
  /// </summary>
  /// <param name="action">the action to reset</param>
  public static void ResetMapping(string action) {
    if (!_originalMappingsCache.ContainsKey(action)) {
      return;
    }

    InputMap.ActionEraseEvents(action);
    var list = _originalMappingsCache[action];
    foreach (var e in list) {
      InputMap.ActionAddEvent(action, e);
    }
    MappingsOverloads.Remove(action);
    _originalMappingsCache.Remove(action);
    OnControlMappingChanged?.Invoke(action);
  }

  private static void LoadMappingsFromData(string action, int[] codes) {
    InputEvent input = null;
    switch (codes[0]) {
      case INPUT_KEY:
        input = new InputEventKey() {
          Keycode = (Key)codes[1]
        };
        break;

      case INPUT_GAMEPAD_BUTTON:
        input = new InputEventJoypadButton() {
          ButtonIndex = (JoyButton)codes[1]
        };
        break;

      case INPUT_MOUSE_BUTTON:
        input = new InputEventMouseButton() {
          ButtonIndex = (MouseButton)codes[1]
        };
        break;
      case INPUT_GAMEPAD_AXIS:
        input = new InputEventJoypadMotion() {
          Axis = (JoyAxis)codes[1],
          AxisValue = codes[2]
        };
        break;
      default:
        break;
    }
    if (input is null) {
      Print.Warn($"Failed to load custom mapping from data:\n\t'{action}' = {codes[0]}:{codes[1]}");
      return;
    }
    SetMapping(action, input);
  }

  private static int[] GetInputCode(InputEvent e) {
    if (e is InputEventKey key) {
      return new int[] { INPUT_KEY, (int)key.Keycode };
    }

    if (e is InputEventJoypadButton btn) {
      return new int[] { INPUT_GAMEPAD_BUTTON, (int)btn.ButtonIndex };
    }

    if (e is InputEventMouseButton mouse) {
      return new int[] { INPUT_MOUSE_BUTTON, (int)mouse.ButtonIndex };
    }

    return e is InputEventJoypadMotion axis ? (new int[] { INPUT_GAMEPAD_AXIS, (int)axis.Axis, (int)axis.AxisValue }) : (new int[] { 0, 0 });
  }

  /// <summary>
  /// Gets the mapping as text for the given action. Or "" if no mapping is found. This can be used to display mappings through text, or be parsed to show button prompts.
  /// </summary>
  /// <param name="action">the action name</param>
  /// <returns>a string of the mapping, where each line is a mapping event parsed as text</returns>
  public static string GetCurrentMappingFor(string action) {
    if (!InputMap.HasAction(action)) {
      Print.Warn($"No action found by name {action}");
      return "";
    };
    var events = InputMap.ActionGetEvents(action);
    var result = "";
    foreach (var e in events) {
      // TODO retrive a simplified mapping string representation. Joypad Axes are especially horrendous
      result += e.AsText() + "\n";
    }
    return result;
  }
  private const string FILE_PATH = "controls.json";

  public static void Load() {
    EventBus.Data.SerializeAll += SaveSettings;

    var builder = new SaveDataBuilder(FILE_PATH, useCurrentSaveSlot: false).LoadFromFile();
    foreach (var entry in builder.Keys) {
      var vector = builder.GetVector3(entry);
      var buffer = new int[] { (int)vector.X, (int)vector.Y, (int)vector.Z };
      LoadMappingsFromData(entry, buffer);
    }
  }

  public static void SaveSettings() {
    var builder = new SaveDataBuilder(FILE_PATH, useCurrentSaveSlot: false);
    foreach (var entry in MappingsOverloads) {
      var x = -1;
      var y = -1;
      var z = -1;
      var buffer = entry.Value;

      // TODO: There's gotta be a better way to write this
      if (buffer.Length > 0) {
        x = entry.Value[0];
      }
      if (buffer.Length > 1) {
        y = entry.Value[1];
      }
      if (buffer.Length > 2) {
        z = entry.Value[2];
      }

      builder.PutVector3("entry.key", new(x, y, z));
    }
  }
}
