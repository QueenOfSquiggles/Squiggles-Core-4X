namespace Squiggles.Core.Scenes.Utility;

using Godot;
using Squiggles.Core.Data;

/// <summary>
/// A tool for driving controller rumble and screen shake from signals and/or animation players (like for cutscenes)
/// </summary>
[GlobalClass]
public partial class EffectsDriver : Node {

  /// <summary>
  /// Requests the controller is rumbled at the specified intensities and for the specified duration
  /// </summary>
  /// <param name="strong">the intensity for the "strong" rumble motors</param>
  /// <param name="weak">the intensity for the "weak" rumble motors</param>
  /// <param name="duration">the length of time in seconds that it should last</param>
  /// <param name="controller_id">If -1, all controllers are rumbled, else the controller bound to the specified ID will be rumbled only. Useful for couch-coop type games</param>
  /// <seealso cref="Effects.Rumble"/>
  public void RumbleController(float strong, float weak, float duration = 0.1f, int controller_id = -1)
      => Effects.Rumble(strong, weak, duration, controller_id);

  /// <summary>
  /// Requests a screen shake
  /// </summary>
  /// <param name="speed">the speed at which to shake (how fast it moves side to side)</param>
  /// <param name="strength">the strength at which to shake (how far it moves from side to side)</param>
  /// <param name="duration">how long, in seconds, for the shaking to last</param>
  public void ShakeScreen(float speed, float strength, float duration)
      => Effects.Shake(speed, strength, duration);

  /// <summary>
  /// Forces the effects to clear in case any effects might be present and are undesired.
  /// </summary>
  public void ClearEffects() {
    RumbleController(0, 0);
    ShakeScreen(0, 0, 0.1f);
  }

}
