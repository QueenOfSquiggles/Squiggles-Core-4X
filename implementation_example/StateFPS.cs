namespace Squiggles.FSMTest;

using Godot;
using Squiggles.Core.Extension;

public partial class StateFPS : PlayerCameraState {

  public override void _UnhandledInput(InputEvent @event) {
    base._UnhandledInput(@event);
    if (!IsActive) {
      return;
    }

    if (@event is InputEventKey iemb
        && iemb.Keycode == Key.Space
        && iemb.IsPressed()) {
      EmitSignal(nameof(OnStateFinished));
      this.HandleInput();
    }
  }



}
