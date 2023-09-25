namespace Squiggles.Core.Scenes.Utility;

using Godot;
using Squiggles.Core.Data;

public partial class EffectsDriver : Node {

  public void RumbleController(float strong, float weak, float duration = 0.1f, int controller_id = -1)
      => Effects.Rumble(strong, weak, duration, controller_id);

  public void ShakeScreen(float speed, float strength, float duration)
      => Effects.Shake(speed, strength, duration);

  public void ClearEffects() {
    RumbleController(0, 0);
    ShakeScreen(0, 0, 0.1f);
  }

}
