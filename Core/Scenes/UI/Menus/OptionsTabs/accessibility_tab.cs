using System;
using Godot;
using queen.data;
using queen.error;
using queen.events;
using queen.extension;

public partial class accessibility_tab : PanelContainer
{
    [Export] private CheckBox _CheckboxNoFlashingLights;
    [Export] private Slider _SliderRumbleStrength;
    [Export] private Slider _SliderScreenShakeStrength;
    [Export] private Slider _SliderRumbleDuration;
    [Export] private Slider _SliderScreenShakeDuration;
    [Export] private Slider _SliderMaxVolume;
    [Export] private Slider _SliderTimeScale;
    [Export] private Slider _SliderGUIScale;
    [Export] private OptionButton _OptionFont;
    [Export] private CheckBox _CheckAlwaysShowReticle;

    private bool _RequiresReload = false;

    public override void _Ready()
    {
        // obligatory workaround for godot not using constructors to load exported values
        if (_CheckboxNoFlashingLights is null ||
            _SliderRumbleStrength is null ||
            _SliderRumbleDuration is null ||
            _SliderScreenShakeStrength is null ||
            _SliderScreenShakeDuration is null ||
            _SliderMaxVolume is null ||
            _SliderTimeScale is null ||
            _OptionFont is null ||
            _SliderGUIScale is null ||
            _CheckAlwaysShowReticle is null
        ) return;

        _CheckboxNoFlashingLights.SetPressedNoSignal(Access.Instance.PreventFlashingLights);
        _SliderRumbleStrength.Value = Effects.Instance.RumbleStrength;
        _SliderRumbleDuration.Value = Effects.Instance.MaxRumbleDuration;
        _SliderScreenShakeStrength.Value = Effects.Instance.ScreenShakeStrength;
        _SliderScreenShakeDuration.Value = Effects.Instance.MaxScreenShakeDuration;
        _SliderMaxVolume.Value = Access.Instance.AudioDecibelLimit;
        _SliderTimeScale.Value = Access.Instance.EngineTimeScale;
        _OptionFont.Selected = Access.Instance.FontOption;
        _SliderGUIScale.Value = Access.Instance.GUI_Scale;
        _CheckAlwaysShowReticle.ButtonPressed = Access.Instance.AlwaysShowReticle;

        _CheckboxNoFlashingLights.Toggled += OnNoFlashingLightsChanged;
        _OptionFont.ItemSelected += OnFontSelected;
        _CheckAlwaysShowReticle.Toggled += OnAlwaysShowReticleToggled;

        _SliderRumbleStrength.ValueChanged += SetRumbleStrength;
        _SliderRumbleDuration.ValueChanged += SetMaxRumbleDuration;
        _SliderScreenShakeStrength.ValueChanged += SetScreenShakeStrength;
        _SliderScreenShakeDuration.ValueChanged += SetMaxScreenShakeDuration;
        _SliderMaxVolume.ValueChanged += SetMaxAudio;
        _SliderTimeScale.ValueChanged += SetEngineTimeScale;
        _SliderGUIScale.ValueChanged += SetGUIScale;

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
            _RequiresReload = true;
        Access.Instance.GUI_Scale = (float)value;
    }

    private void OnFontSelected(long index)
    {
        var target = _OptionFont?.GetItemId((int)index) ?? Access.FONT_GOTHIC;
        if (target == Access.Instance.FontOption) return;

        Access.Instance.FontOption = target;
        _RequiresReload = true;
    }

    private void OnAlwaysShowReticleToggled(bool buttonPressed)
        => Access.Instance.AlwaysShowReticle = buttonPressed;


    public void ApplyChanges()
    {
        Access.SaveSettings();
        Effects.SaveSettings();
        if (_RequiresReload) GetTree().ReloadCurrentScene(); // no need to set variable. reload resets everything
    }
}
