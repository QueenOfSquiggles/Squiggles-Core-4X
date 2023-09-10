using System;
using Godot;
using queen.data;
using queen.error;
using queen.events;
using queen.extension;

public partial class accessibility_tab : PanelContainer
{
    [Export] private NodePath path_checkbox_no_flashing_lights;
    [Export] private NodePath path_slider_rumble_strength;
    [Export] private NodePath path_slider_screen_shake_strength;
    [Export] private NodePath path_slider_rumble_duration;
    [Export] private NodePath path_slider_screen_shake_duration;
    [Export] private NodePath path_slider_max_volume;
    [Export] private NodePath path_slider_engine_time_scale;
    [Export] private NodePath path_option_font;
    [Export] private NodePath path_gui_scale;
    [Export] private NodePath path_check_always_show_reticle;

    private CheckBox checkbox_no_flashing_lights;
    private Slider slider_rumble_strength;
    private Slider slider_screen_shake_strength;
    private Slider slider_rumble_duration;
    private Slider slider_screen_shake_duration;
    private Slider slider_max_volume;
    private Slider slider_time_scale;
    private Slider slider_gui_scale;
    private OptionButton option_font;
    private CheckBox check_always_show_reticle;

    private bool RequiresReload = false;

    public override void _Ready()
    {
        this.GetNode(path_checkbox_no_flashing_lights, out checkbox_no_flashing_lights);
        this.GetNode(path_option_font, out option_font);
        this.GetNode(path_check_always_show_reticle, out check_always_show_reticle);
        // The Slider Combo needs to be 'cracked' to access the actual slider node. Not preferable...
        // TODO maybe find a better way to access this node? 
        this.GetSafe(path_slider_rumble_strength, out slider_rumble_strength);
        this.GetSafe(path_slider_screen_shake_strength, out slider_screen_shake_strength);
        this.GetSafe(path_slider_rumble_duration, out slider_rumble_duration);
        this.GetSafe(path_slider_screen_shake_duration, out slider_screen_shake_duration);
        this.GetSafe(path_slider_max_volume, out slider_max_volume);
        this.GetSafe(path_slider_engine_time_scale, out slider_time_scale);
        this.GetSafe(path_gui_scale, out slider_gui_scale);

        checkbox_no_flashing_lights.SetPressedNoSignal(Access.Instance.PreventFlashingLights);
        slider_rumble_strength.Value = Effects.Instance.RumbleStrength;
        slider_rumble_duration.Value = Effects.Instance.MaxRumbleDuration;
        slider_screen_shake_strength.Value = Effects.Instance.ScreenShakeStrength;
        slider_screen_shake_duration.Value = Effects.Instance.MaxScreenShakeDuration;
        slider_max_volume.Value = Access.Instance.AudioDecibelLimit;
        slider_time_scale.Value = Access.Instance.EngineTimeScale;
        option_font.Selected = Access.Instance.FontOption;
        slider_gui_scale.Value = Access.Instance.GUI_Scale;
        check_always_show_reticle.ButtonPressed = Access.Instance.AlwaysShowReticle;

        checkbox_no_flashing_lights.Toggled += OnNoFlashingLightsChanged;
        option_font.ItemSelected += OnFontSelected;
        check_always_show_reticle.Toggled += OnAlwaysShowReticleToggled;

        slider_rumble_strength.ValueChanged += SetRumbleStrength;
        slider_rumble_duration.ValueChanged += SetMaxRumbleDuration;
        slider_screen_shake_strength.ValueChanged += SetScreenShakeStrength;
        slider_screen_shake_duration.ValueChanged += SetMaxScreenShakeDuration;
        slider_max_volume.ValueChanged += SetMaxAudio;
        slider_time_scale.ValueChanged += SetEngineTimeScale;
        slider_gui_scale.ValueChanged += SetGUIScale;

        Events.Data.SerializeAll += ApplyChanges;

    }


    public override void _ExitTree()
    {
        Events.Data.SerializeAll -= ApplyChanges;
    }

    private void OnNoFlashingLightsChanged(bool do_no_flashing_lights)
        => Access.Instance.PreventFlashingLights = do_no_flashing_lights;
    private void SetRumbleStrength(double value)
        => Effects.Instance.RumbleStrength = (float)value;
    private void SetScreenShakeStrength(double value)
        => Effects.Instance.ScreenShakeStrength = (float)value;
    private void SetMaxRumbleDuration(double value)
        => Effects.Instance.MaxRumbleDuration = (float)value;
    private void SetMaxScreenShakeDuration(double value)
        => Effects.Instance.MaxScreenShakeDuration = (float)value;
    private void SetMaxAudio(double value)
        => Access.Instance.AudioDecibelLimit = (float)value;
    private void SetEngineTimeScale(double value)
        => Access.Instance.EngineTimeScale = (float)value;

    private void SetGUIScale(double value)
    {
        if (Access.Instance.GUI_Scale != (float)value)
            RequiresReload = true;
        Access.Instance.GUI_Scale = (float)value;
    }

    private void OnFontSelected(long index)
    {
        var target = option_font.GetItemId((int)index);
        if (target == Access.Instance.FontOption) return;

        Access.Instance.FontOption = target;
        RequiresReload = true;
    }

    private void OnAlwaysShowReticleToggled(bool buttonPressed)
        => Access.Instance.AlwaysShowReticle = buttonPressed;


    public void ApplyChanges()
    {
        Access.SaveSettings();
        Effects.SaveSettings();
        if (RequiresReload) GetTree().ReloadCurrentScene(); // no need to set variable. reload resets everything
    }
}
