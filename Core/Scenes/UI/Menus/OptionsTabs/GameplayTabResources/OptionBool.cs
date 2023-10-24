namespace Squiggles.Core.Scenes.UI.Menus.Gameplay;

using Godot;


/// <summary>
/// An <see cref="OptionBase"/> that stores a bool value
/// </summary>
[GlobalClass]
public partial class OptionBool : OptionBase {
  /// <summary>
  /// The stored value of the option.
  /// </summary>
  [Export] public bool Value;
}
