namespace Squiggles.Core.Scenes.World;

using Godot;

public partial class CutsceneSkipper : Node {

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

  public void Start() => _isListening = true;
  public void Stop() => _isListening = false;

  private void SkipCutscene() => _anim?.Seek(_anim.CurrentAnimationLength, true);

}
