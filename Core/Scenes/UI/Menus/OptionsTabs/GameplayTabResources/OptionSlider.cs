namespace Squiggles.Core.Scenes.UI.Menus.Gameplay;

using Godot;

/// <summary>
/// An <see cref="OptionBase"/> which stores a number (System.Single). It displays as a slider in menus and as such requires several values used by <see cref="Range"/>
/// </summary>
[GlobalClass]
public partial class OptionSlider : OptionBase {

  /// <summary>
  /// The default value of this option. Must be within the bounds specified otherwise
  /// </summary>
  [Export] public float DefaultValue = 0.5f;
  /// <summary>
  /// The minimum value that this option is allowed to be
  /// </summary>
  [Export] public float MinValue = 0.0f;
  /// <summary>
  /// The maximum value that this value is allowed to be
  /// </summary>
  [Export] public float MaxValue = 1.0f;
  /// <summary>
  /// The amont to step by when using the slider. Set to 1.0 to effectively make this an integer. By default it is quite small to allow floats to be very granular
  /// </summary>
  [Export] public float StepValue = 0.001f;
  /// <summary>
  /// The Range option which disables the <see cref="MinValue"/> allowing the stored value to be infinitely lesser than what would be allowed otherwise.
  /// </summary>
  [Export] public bool AllowLesser = false;
  /// <summary>
  /// The Range option which disables the <see cref="MaxValue"/> allowing the stored value to be infinitely greater than what would be allowed otherwise.
  /// </summary>
  [Export] public bool AllowGreater = false;

}
