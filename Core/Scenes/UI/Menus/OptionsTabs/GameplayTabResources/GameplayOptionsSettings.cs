namespace Squiggles.Core.Scenes.UI.Menus.Gameplay;

using Godot;

/// <summary>
/// The resource used for storying gameplay options. Highly dynamic and loaded from the configuration file.
/// </summary>
[GlobalClass]
public partial class GameplayOptionsSettings : Resource {
  /// <summary>
  /// What the default mouse mode is for gameplay. FPS games usually want <c>Input.MouseModeEnum.Captured</c> whereas more GUI focused games might prefer <c>Input.MouseModeEnum.Visible</c>
  /// </summary>
  [Export] public Input.MouseModeEnum GameplayMouseMode = Input.MouseModeEnum.Captured;
  /// <summary>
  /// An array of <see cref="OptionBase"/>s that are to be loaded into the gameplay options menu tab
  /// </summary>
  [Export] public OptionBase[] OptionsArray;

}
