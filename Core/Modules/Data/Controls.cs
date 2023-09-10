using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using queen.error;

namespace queen.data;

public class Controls
{

    //
    //  Meaningful information
    //      Defaults assigned as well
    //

    public float MouseLookSensivity = 400.0f;
    public float ControllerLookSensitivity = 500.0f;

    public Dictionary<string, int[]> mappings_overloads = new();
    private Dictionary<string, List<InputEvent>> original_mappings_cache = new();


    //
    //  Events
    //

    public event Action<string> OnControlMappingChanged;

    //
    //
    //
    private const int INPUT_KEY = 1;
    private const int INPUT_GAMEPAD_BUTTON = 2;
    private const int INPUT_MOUSE_BUTTON = 3;
    private const int INPUT_GAMEPAD_AXIS = 4;

    public void ResetMappings()
    {
        var affected = new List<string>();
        foreach (var key in mappings_overloads.Keys)
        {
            affected.Add(key);
        }
        InputMap.LoadFromProjectSettings();
        mappings_overloads.Clear();
        foreach (var action in affected)
        {
            OnControlMappingChanged?.Invoke(action);
        }
    }
    public void SetMapping(string action, InputEvent assigned_input)
    {
        if (!InputMap.HasAction(action))
        {
            Print.Warn($"Failed to assign event {assigned_input.AsText()} to input action '{action}'. Mapping not found in InputMap");
            return;
        }

        if (!original_mappings_cache.ContainsKey(action))
        {
            // caches the originally loaded mapping
            var event_list = InputMap.ActionGetEvents(action);
            var list = new List<InputEvent>();
            list.AddRange(event_list.AsEnumerable());
            original_mappings_cache.Add(action, list);
        }

        InputMap.ActionEraseEvents(action);
        InputMap.ActionAddEvent(action, assigned_input);

        mappings_overloads[action] = GetInputCode(assigned_input);
        OnControlMappingChanged?.Invoke(action);
    }

    public void ResetMapping(string action)
    {
        if (!original_mappings_cache.ContainsKey(action)) return;
        InputMap.ActionEraseEvents(action);
        var list = original_mappings_cache[action];
        foreach (var e in list)
        {
            InputMap.ActionAddEvent(action, e);
        }
        mappings_overloads.Remove(action);
        original_mappings_cache.Remove(action);
        OnControlMappingChanged?.Invoke(action);
    }

    private void LoadMappingsFromData(string action, int[] codes)
    {
        InputEvent input = null;
        switch (codes[0])
        {
            case INPUT_KEY:
                input = new InputEventKey()
                {
                    Keycode = (Key)codes[1]
                };
                break;

            case INPUT_GAMEPAD_BUTTON:
                input = new InputEventJoypadButton()
                {
                    ButtonIndex = (JoyButton)codes[1]
                };
                break;

            case INPUT_MOUSE_BUTTON:
                input = new InputEventMouseButton()
                {
                    ButtonIndex = (MouseButton)codes[1]
                };
                break;
            case INPUT_GAMEPAD_AXIS:
                input = new InputEventJoypadMotion()
                {
                    Axis = (JoyAxis)codes[1],
                    AxisValue = codes[2]
                };
                break;
        }
        if (input == null)
        {
            Print.Warn($"Failed to load custom mapping from data:\n\t'{action}' = {codes[0]}:{codes[1]}");
            return;
        }
        SetMapping(action, input);
    }

    private int[] GetInputCode(InputEvent e)
    {
        if (e is InputEventKey key) return new int[] { INPUT_KEY, (int)key.Keycode };
        if (e is InputEventJoypadButton btn) return new int[] { INPUT_GAMEPAD_BUTTON, (int)btn.ButtonIndex };
        if (e is InputEventMouseButton mouse) return new int[] { INPUT_MOUSE_BUTTON, (int)mouse.ButtonIndex };
        if (e is InputEventJoypadMotion axis) return new int[] { INPUT_GAMEPAD_AXIS, (int)axis.Axis, (int)axis.AxisValue };
        return new int[] { 0, 0 };
    }

    public string GetCurrentMappingFor(string action)
    {
        if (!InputMap.HasAction(action))
        {
            Print.Warn($"No action found by name {action}");
            return "";
        };
        var events = InputMap.ActionGetEvents(action);
        string result = "";
        foreach (var e in events)
        {
            // TODO retrive a simplified mapping string representation. Joypad Axes are especially horrendous
            result += e.AsText() + "\n";
        }
        return result;
    }

    //
    //  Singleton Setup
    //
    public static Controls Instance
    {
        get
        {
            if (_instance == null) CreateInstance();
            return _instance;
        }
    }
    private static Controls _instance = null;
    private const string FILE_PATH = "controls.json";

    private static void CreateInstance()
    {
        _instance = new Controls();
        var loaded = Data.Load<Controls>(FILE_PATH);
        if (loaded != null) _instance = loaded;
        foreach (var key in _instance.mappings_overloads.Keys)
        {
            var codes = _instance.mappings_overloads[key];
            _instance.LoadMappingsFromData(key, codes);
        }
    }

    public static void SaveSettings()
    {
        if (_instance == null) return;
        Data.Save(_instance, FILE_PATH);
    }
}