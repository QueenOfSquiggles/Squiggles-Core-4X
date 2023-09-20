namespace Squiggles.Core.Scenes.UI.Menus.Gameplay;

using Godot;

[GlobalClass]
public partial class OptionComboSelect : OptionBase {

  [Export] public string[] Options;
  [Export] public int DefaultSelection;
}
