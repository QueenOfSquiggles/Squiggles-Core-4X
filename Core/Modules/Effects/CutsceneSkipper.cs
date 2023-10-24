namespace Squiggles.Core.Scenes.World;

using Godot;

/// <summary>
/// A handle tool for allowing players to skip any animation sequence. "skipping" actually uses the animation player's "seek" function which ensures all animation modifications are still applied. So any functions called, events triggered, or manipulation of transforms is respected with this skipper.
/// </summary>
[GlobalClass]
public partial class CutsceneSkipper : Node {

  /// <summary>
  /// A reference to the animation player that is allowing skipping
  /// </summary>
  [Export] private AnimationPlayer _anim;

  private bool _isListening;

  public override void _Input(InputEvent @event) {
    if (!_isListening) {
      return;
    }
    // any button input (unlikely to have noisy input) skips the cutscene
    if (@event is InputEventKey or InputEventJoypadButton) {
      SkipCutscene();
    }
  }

  /// <summary>
  /// Marks that this skipper is active and should listen for key or gamepad button inputs.
  /// </summary>
  public void Start() => _isListening = true;
  /// <summary>
  /// Marks that this skipper is not active and should not listen for key or gamepad button inputs.
  /// </summary>
  public void Stop() => _isListening = false;

  private void SkipCutscene() => _anim?.Seek(_anim.CurrentAnimationLength, true);

}
