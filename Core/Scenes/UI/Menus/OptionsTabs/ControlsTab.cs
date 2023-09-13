using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using queen.data;
using queen.error;
using queen.events;
using queen.extension;

public partial class ControlsTab : PanelContainer
{

    private bool _Listening = false;
    private string _CurrentActionTarget = "";
    [Export] private Popup _PopupListening;
    [Export] private Slider _SliderMouse;
    [Export] private Slider SliderGamepad;

    [ExportGroup("Mappings", "_Mapping")]
    [Export] private Control _MappingRoot;
    [Export] private PackedScene _MappingScene;

    public override void _Ready()
    {
        if (_PopupListening is null ||
            _PopupListening is null ||
            _SliderMouse is null ||
            SliderGamepad is null
        ) return;
        _PopupListening.Exclusive = true;
        _PopupListening.WindowInput += _Input;

        _SliderMouse.Value = Controls.Instance.MouseLookSensivity;
        SliderGamepad.Value = Controls.Instance.ControllerLookSensitivity;

        Events.Data.SerializeAll += ApplyChanges;

        var keys = ThisIsYourMainScene.Config?.RemapControlsNames ?? Array.Empty<string>();
        if (keys.Length <= 0)
        {
            // empty array, assume all are valid and place custom mappings first
            var mappings = InputMap.GetActions();
            var union = new List<StringName>();
            var custom_mappings = mappings.Where((key) => !key.ToString().StartsWith("ui")).ToList();
            union.AddRange(custom_mappings);
            if (!(ThisIsYourMainScene.Config?.HideUIMappings ?? true))
            {
                var ui_mappings = mappings.Where((key) => key.ToString().StartsWith("ui")).ToList();
                union.AddRange(ui_mappings);
            }

            keys = new string[union.Count];
            for (int i = 0; i < union.Count; i++) keys[i] = union[i];
        }
        foreach (var action in keys)
        {
            var scene = _MappingScene?.Instantiate() as ActionMappingSlot;
            if (scene is null) continue;
            scene.Name = $"Remam_{action}";
            scene.TargetAction = action;
            _MappingRoot?.AddChild(scene);
            scene.ListenForAction += ListenForAction;
        }
    }

    public override void _ExitTree()
    {
        Events.Data.SerializeAll -= ApplyChanges;
    }

    public async void ListenForAction(string action_name)
    {
        _CurrentActionTarget = action_name;
        _PopupListening?.PopupCenteredRatio();
        await Task.Delay(50);
        _Listening = true;
    }

    public override void _Input(InputEvent e)
    {
        if (!_Listening) return;
        if (_CurrentActionTarget.Length == 0) return;

        bool is_valid = false;

        // OMFG I'm loving pattern matching!!!
        if (e is InputEventKey key) is_valid = key.Pressed;
        else if (e is InputEventJoypadButton joy) is_valid = joy.Pressed;
        else if (e is InputEventMouseButton mou) is_valid = mou.Pressed;
        else if (e is InputEventJoypadMotion axis) is_valid = Mathf.Abs(axis.AxisValue) > 0.999f; // force max to avoid noise/drift

        if (is_valid)
        {
            Print.Debug($"Processing input event override for action {_CurrentActionTarget}, received event: {e.AsText()}");
            Controls.Instance.SetMapping(_CurrentActionTarget, e);
            _CurrentActionTarget = "";
            _Listening = false;
            _PopupListening?.Hide();
        }
    }

    public void ResetAllMappings()
    {
        Controls.Instance.ResetMappings();
    }

    public void ApplyChanges()
    {
        Controls.Instance.MouseLookSensivity = (float)(_SliderMouse?.Value ?? 0);
        Controls.Instance.ControllerLookSensitivity = (float)(SliderGamepad?.Value ?? 0);
        Controls.SaveSettings();
    }

}
