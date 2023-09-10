using System;
using System.Threading.Tasks;
using Godot;
using queen.data;
using queen.error;
using queen.events;
using queen.extension;

public partial class ControlsTab : PanelContainer
{

    [Export] private NodePath PathSliderMouseSensitive;
    [Export] private NodePath PathSliderGamepadSensitive;
    [Export] private NodePath PathListeningPopup;

    private bool Listening = false;
    private string CurrentActionTarget = "";
    private Popup popup_listening;

    private Slider SliderMouse;
    private Slider SliderGamepad;

    public override void _Ready()
    {
        this.GetNode(PathListeningPopup, out popup_listening);
        this.GetSafe(PathSliderMouseSensitive, out SliderMouse);
        this.GetSafe(PathSliderGamepadSensitive, out SliderGamepad);
        popup_listening.Exclusive = true;
        popup_listening.WindowInput += _Input;

        SliderMouse.Value = Controls.Instance.MouseLookSensivity;
        SliderGamepad.Value = Controls.Instance.ControllerLookSensitivity;

        Events.Data.SerializeAll += ApplyChanges;
    }

    public override void _ExitTree()
    {
        Events.Data.SerializeAll -= ApplyChanges;
    }

    public async void ListenForAction(string action_name)
    {
        CurrentActionTarget = action_name;
        popup_listening.PopupCenteredRatio();
        await Task.Delay(50);
        Listening = true;
    }

    public override void _Input(InputEvent e)
    {
        if (!Listening) return;
        if (CurrentActionTarget.Length == 0) return;

        bool is_valid = false;

        // OMFG I'm loving pattern matching!!!
        if (e is InputEventKey key) is_valid = key.Pressed;
        else if (e is InputEventJoypadButton joy) is_valid = joy.Pressed;
        else if (e is InputEventMouseButton mou) is_valid = mou.Pressed;
        else if (e is InputEventJoypadMotion axis) is_valid = Mathf.Abs(axis.AxisValue) > 0.999f; // force max to avoid noise/drift

        if (is_valid)
        {
            Print.Debug($"Processing input event override for action {CurrentActionTarget}, received event: {e.AsText()}");
            Controls.Instance.SetMapping(CurrentActionTarget, e);
            CurrentActionTarget = "";
            Listening = false;
            popup_listening.Hide();
        }
    }

    public void ResetAllMappings()
    {
        Controls.Instance.ResetMappings();
    }

    public void ApplyChanges()
    {
        Controls.Instance.MouseLookSensivity = (float)SliderMouse.Value;
        Controls.Instance.ControllerLookSensitivity = (float)SliderGamepad.Value;
        Controls.SaveSettings();
    }

}
