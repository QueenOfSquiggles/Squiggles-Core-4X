namespace Squiggles.Core.Scenes.UI.Menus.Gameplay;


using Godot;

/// <summary>
/// The base of an option for the <see cref="GameplayOptionsSettings"/>. The child classes of this are able to be applied to create custom options that are generated in the menu and are able to be retrieved using <see cref="GameplaySettings"/>
/// </summary>
[GlobalClass]
public partial class OptionBase : Resource {
  /// <summary>
  /// The name to diplay in menu (can be a translation id for internaltionalization)
  /// </summary>
  [Export] public string InMenuName = "Option";
  /// <summary>
  /// The name that serves as the key for this option internally. Must be unique among other OptionBase values, but otherwise it can be literally anything you want.
  /// </summary>
  [Export] public string InternalName = "option";
}
