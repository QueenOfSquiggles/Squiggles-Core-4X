namespace Squiggles.Core.Scenes.Utility.Camera;

using Godot;
using Squiggles.Core.Events;

public partial class CinematicSequence : Node {

  public void Start() => EventBus.Gameplay.TriggerRequestPlayerAbleToMove(false);

  public void End() => EventBus.Gameplay.TriggerRequestPlayerAbleToMove(true);
}
