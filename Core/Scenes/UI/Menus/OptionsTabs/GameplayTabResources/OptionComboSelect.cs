namespace Squiggles.Core.Scenes.UI.Menus.Gameplay;

using Godot;

/// <summary>
/// An <see cref="OptionBase"/> that stores a collection of strings as well as the index that is selected. This is used when you want to have multiple options for a single value. It does require some additional parsing, but it should be easier than starting from nothing!
/// </summary>
[GlobalClass]
public partial class OptionComboSelect : OptionBase {

  /// <summary>
  /// The array of options. These are simultaneously the value shown in-menu as well as the values stored in <see cref="GameplaySettings"/>
  /// </summary>
  [Export] public string[] Options;
  /// <summary>
  /// The default index to select when no data is loaded from disk.
  /// </summary>
  [Export] public int DefaultSelection;
}
