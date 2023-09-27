namespace Squiggles.FSMTest;

using Godot;
using Squiggles.Core.Extension;

public partial class StateOTS : PlayerCameraState {

  [Signal] public delegate void OnReleaseMouseEventHandler();

  public override void _UnhandledInput(InputEvent @event) {
    base._UnhandledInput(@event);
    if (!IsActive) {
      return;
    }

    if (@event is InputEventMouseButton iemb
        && iemb.ButtonIndex == MouseButton.Right
        && iemb.IsReleased()) {

      EmitSignal(nameof(OnReleaseMouse));
      this.HandleInput();
    }

    if (@event is InputEventKey iek
        && iek.Keycode == Key.Space
        && iek.IsPressed()) {

      EmitSignal(nameof(OnStateFinished));
      this.HandleInput();
    }
  }
}
