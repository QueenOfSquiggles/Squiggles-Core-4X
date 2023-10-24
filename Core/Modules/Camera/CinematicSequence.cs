namespace Squiggles.Core.Scenes.Utility.Camera;

using Godot;
using Squiggles.Core.Events;

/// <summary>
/// A utility nonde that enables creating a cinematic sequence where the player is unable to move temporarily.
/// This is accomplished by calling the methods <see cref="Start"/> and <see cref="End"/> respectively.
/// They both route to <see cref="EventBus.Gameplay.TriggerRequestPlayerAbleToMove"/>
/// </summary>
[GlobalClass]
public partial class CinematicSequence : Node {

  /// <summary>
  /// Marks the cinematic sequence to have started, meaning the player should no longer be allowed to move
  /// </summary>
  public void Start() => EventBus.Gameplay.TriggerRequestPlayerAbleToMove(false);

  /// <summary>
  /// Marks the cinematic sequence to have ended, which allows to player to move again.
  /// </summary>
  public void End() => EventBus.Gameplay.TriggerRequestPlayerAbleToMove(true);
}
