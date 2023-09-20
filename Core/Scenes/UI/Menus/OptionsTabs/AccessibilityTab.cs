namespace Squiggles.Core.Scenes.UI.Menus;

using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Events;

public partial class AccessibilityTab : PanelContainer {
  [Export] private CheckBox _checkboxNoFlashingLights;
  [Export] private Slider _sliderRumbleStrength;
  [Export] private Slider _sliderScreenShakeStrength;
  [Export] private Slider _sliderRumbleDuration;
  [Export] private Slider _sliderScreenShakeDuration;
  [Export] private Slider _sliderMaxVolume;
  [Export] private Slider _sliderTimeScale;
  [Export] private Slider _sliderGUIScale;
  [Export] private OptionButton _optionFont;
  [Export] private CheckBox _checkAlwaysShowReticle;

  private bool _requiresReload;

  public override void _Ready() {
    _checkboxNoFlashingLights.SetPressedNoSignal(Access.Instance.PreventFlashingLights);
    _sliderRumbleStrength.Value = Effects.Instance.RumbleStrength;
    _sliderRumbleDuration.Value = Effects.Instance.MaxRumbleDuration;
    _sliderScreenShakeStrength.Value = Effects.Instance.ScreenShakeStrength;
    _sliderScreenShakeDuration.Value = Effects.Instance.MaxScreenShakeDuration;
    _sliderMaxVolume.Value = Access.Instance.AudioDecibelLimit;
    _sliderTimeScale.Value = Access.Instance.EngineTimeScale;
    _optionFont.Selected = Access.Instance.FontOption;
    _sliderGUIScale.Value = Access.Instance.GUI_Scale;
    _checkAlwaysShowReticle.ButtonPressed = Access.Instance.AlwaysShowReticle;

    _checkboxNoFlashingLights.Toggled += OnNoFlashingLightsChanged;
    _optionFont.ItemSelected += OnFontSelected;
    _checkAlwaysShowReticle.Toggled += OnAlwaysShowReticleToggled;

    _sliderRumbleStrength.ValueChanged += SetRumbleStrength;
    _sliderRumbleDuration.ValueChanged += SetMaxRumbleDuration;
    _sliderScreenShakeStrength.ValueChanged += SetScreenShakeStrength;
    _sliderScreenShakeDuration.ValueChanged += SetMaxScreenShakeDuration;
    _sliderMaxVolume.ValueChanged += SetMaxAudio;
    _sliderTimeScale.ValueChanged += SetEngineTimeScale;
    _sliderGUIScale.ValueChanged += SetGUIScale;

    EventBus.Data.SerializeAll += ApplyChanges;

  }


  public override void _ExitTree() => EventBus.Data.SerializeAll -= ApplyChanges;

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

  private void SetGUIScale(double value) {
    if (Access.Instance.GUI_Scale != (float)value) {
      _requiresReload = true;
    }

    Access.Instance.GUI_Scale = (float)value;
  }

  private void OnFontSelected(long index) {
    var target = _optionFont?.GetItemId((int)index) ?? Access.FONT_GOTHIC;
    if (target == Access.Instance.FontOption) {
      return;
    }

    Access.Instance.FontOption = target;
    _requiresReload = true;
  }

  private void OnAlwaysShowReticleToggled(bool buttonPressed)
      => Access.Instance.AlwaysShowReticle = buttonPressed;


  public void ApplyChanges() {
    Access.SaveSettings();
    Effects.SaveSettings();
    if (_requiresReload) {
      GetTree().ReloadCurrentScene(); // no need to set variable. reload resets everything
    }
  }
}
