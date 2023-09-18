namespace Squiggles.Core.Scenes.UI.Menus.Gameplay;

using Godot;

[GlobalClass]
public partial class GameplayOptionsSettings : Resource {
  [Export] public Input.MouseModeEnum GameplayMouseMode = Input.MouseModeEnum.Captured;
  [Export] public OptionBase[] OptionsArray;

}
