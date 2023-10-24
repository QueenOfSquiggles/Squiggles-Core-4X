namespace Squiggles.Core.Scenes.UI.Menus;

using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Events;

/// <summary>
/// This is the "tab" for accessibility settings in the options menu. It stores all accessibility related features. These settings are reflected in the <see cref="Access"/> singleton. Also check there if you want more details on what the different settings are for
/// </summary>
/// <remarks>
/// One of the goals for this framework is to have a large number of accessibility features available out of the box so devs can make their games more accessible. That being said, I led you to the water. Are you gonna drink anytime soon? (or maybe you have already, in that case you get soft headpats XOXO)
/// <para/>
/// In the before times (QueenOfSquiggles's "Where The Dead Lie") the options menu was actually a tab container. However that style was incredibly bulky and required a ton of workarounds to ensure full gamepad support.
/// </remarks>
public partial class AccessibilityTab : PanelContainer {
  /// <summary>
  /// The checkbox for preventing flashing lights. This does require developer implementation, but it is easily provided in <see cref="Access"/>
  /// </summary>
  /// <remarks>
  /// This feature is built in by default as I usually make horror games and one of my biggest pet peeve is horror game devs that have no consideration for accessibility. I have several family members who are susceptible to seizures and one of them even is a huge fan of horror. So this is a *not so subtle* way to encourage horror game developers (or really any dev) to either reconsider whether they actually need flashing lights to make their game "scary", or at least implement it in a way that allows those with sensitivities to protect themselves. Video Games are a lot like BDSM, things get really bad really quick if both parties are consenting to what's happening.
  /// </remarks>
  [Export] private CheckBox _checkboxNoFlashingLights;
  /// <summary>
  /// The slider for the rumble strength limiter
  /// </summary>
  [Export] private Slider _sliderRumbleStrength;
  /// <summary>
  /// The slider for the screen shake strength limiter
  /// </summary>
  [Export] private Slider _sliderScreenShakeStrength;
  /// <summary>
  /// the slider for the rumble duration limiter
  /// </summary>
  [Export] private Slider _sliderRumbleDuration;
  /// <summary>
  /// The slider for the screen shake duration limiter
  /// </summary>
  [Export] private Slider _sliderScreenShakeDuration;
  /// <summary>
  /// The slider for the volume limiter
  /// </summary>
  [Export] private Slider _sliderMaxVolume;
  /// <summary>
  /// The slider for the engine time scale
  /// </summary>
  [Export] private Slider _sliderTimeScale;
  /// <summary>
  /// The slider for the GUI scale
  /// </summary>
  [Export] private Slider _sliderGUIScale;
  /// <summary>
  /// The option button (combo box is more familair to me LOL) for which font to use.
  /// </summary>
  [Export] private OptionButton _optionFont;
  /// <summary>
  /// The checkbox for always showing the reticle.
  /// </summary>
  [Export] private CheckBox _checkAlwaysShowReticle;

  private bool _requiresReload;

  public override void _Ready() {
    _checkboxNoFlashingLights.SetPressedNoSignal(Access.PreventFlashingLights);
    _sliderRumbleStrength.Value = Effects.RumbleStrength;
    _sliderRumbleDuration.Value = Effects.MaxRumbleDuration;
    _sliderScreenShakeStrength.Value = Effects.ScreenShakeStrength;
    _sliderScreenShakeDuration.Value = Effects.MaxScreenShakeDuration;
    _sliderMaxVolume.Value = Access.AudioDecibelLimit;
    _sliderTimeScale.Value = Access.EngineTimeScale;
    _optionFont.Selected = Access.FontOption;
    _sliderGUIScale.Value = Access.GUI_Scale;
    _checkAlwaysShowReticle.ButtonPressed = Access.AlwaysShowReticle;

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
      => Access.PreventFlashingLights = do_no_flashing_lights;
  private void SetRumbleStrength(double value)
      => Effects.RumbleStrength = (float)value;
  private void SetScreenShakeStrength(double value)
      => Effects.ScreenShakeStrength = (float)value;
  private void SetMaxRumbleDuration(double value)
      => Effects.MaxRumbleDuration = (float)value;
  private void SetMaxScreenShakeDuration(double value)
      => Effects.MaxScreenShakeDuration = (float)value;
  private void SetMaxAudio(double value)
      => Access.AudioDecibelLimit = (float)value;
  private void SetEngineTimeScale(double value)
      => Access.EngineTimeScale = (float)value;

  private void SetGUIScale(double value) {
    if (Access.GUI_Scale != (float)value) {
      _requiresReload = true;
    }

    Access.GUI_Scale = (float)value;
  }

  private void OnFontSelected(long index) {
    var target = _optionFont?.GetItemId((int)index) ?? Access.FONT_GOTHIC;
    if (target == Access.FontOption) {
      return;
    }

    Access.FontOption = target;
    _requiresReload = true;
  }

  private void OnAlwaysShowReticleToggled(bool buttonPressed)
      => Access.AlwaysShowReticle = buttonPressed;


  public void ApplyChanges() {
    Access.SaveSettings();
    Effects.SaveSettings();
    if (_requiresReload) {
      // reloading scene breaks some things???
      // GetTree().ReloadCurrentScene(); // no need to set variable. reload resets everything
    }
  }
}
