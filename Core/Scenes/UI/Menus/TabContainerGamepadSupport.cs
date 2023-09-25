namespace Squiggles.Core.Scenes.UI.Menus;

using Godot;
using Squiggles.Core.Extension;

/// <summary>
/// A utility class that enables navigating tabcontainers using a gamepad. No longer used internally, but could be quite useful
/// </summary>
[GlobalClass]
public partial class TabContainerGamepadSupport : TabContainer {
  public override void _UnhandledInput(InputEvent @event) {
    if (@event.IsActionPressed("ui_cycle_left")) {

      if (CurrentTab == 0) {
        CurrentTab = GetChildCount() - 1;
      }
      else {
        CurrentTab--;
      }

      this.HandleInput();
    }
    if (@event.IsActionPressed("ui_cycle_right")) {
      if (CurrentTab == (GetChildCount() - 1)) {
        CurrentTab = 0;
      }
      else {
        CurrentTab++;
      }

      this.HandleInput();
    }
  }
}
