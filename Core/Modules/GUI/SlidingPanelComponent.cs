namespace Squiggles.Core.Scenes.UI;

using System.Threading.Tasks;
using Godot;
using Squiggles.Core.Extension;

/// <summary>
/// A component used heavily in the main menu elements. It is designed to create cascading panels that slide in and out smoothly. See the default main menu for examples.
/// </summary>
[GlobalClass]
public partial class SlidingPanelComponent : Node {

  /// <summary>
  /// The time this panel takes to slide in
  /// </summary>
  [Export] private float _popInDuration = 0.3f;
  /// <summary>
  /// The time this panel takes to slide out
  /// </summary>
  [Export] private float _popOutDuration = 0.3f;
  /// <summary>
  ///
  /// </summary>
  [Export] private Control _target;
  /// <summary>
  /// The path where child sliding panels should be added
  /// </summary>
  [Export] private NodePath _pathSubSlidingRoot = "..";
  /// <summary>
  /// The transition type to use when showing
  /// </summary>
  [Export] private Tween.TransitionType _transShow = Tween.TransitionType.Cubic;
  /// <summary>
  /// The transition type to use when hiding
  /// </summary>
  [Export] private Tween.TransitionType _transHide = Tween.TransitionType.Cubic;
  /// <summary>
  /// The ease type to use when showing
  /// </summary>
  [Export] private Tween.EaseType _easingShow = Tween.EaseType.Out;
  /// <summary>
  /// The ease type to use when hiding
  /// </summary>
  [Export] private Tween.EaseType _easingHide = Tween.EaseType.Out;

  private bool _isStable; // mood

  public override void _Ready() {
    _target ??= GetNode<Control>("..");
    _target.ZIndex -= 1;
    var xStart = _target.Position.X;
    _target.Position -= new Vector2(_target.Size.X, 0.0f);
    var tween = CreateTween().SetTrans(_transShow).SetEase(_easingShow);
    tween.TweenProperty(_target, "position:x", xStart, _popInDuration);
    tween.TweenProperty(this, nameof(_isStable), true, 0.01f);
  }

  public override void _UnhandledInput(InputEvent @event) {
    if (!@event.IsActionPressed("ui_cancel") || !_isStable) {
      return;
    }

    _ = RemoveScene();
  }

  /// <summary>
  /// An async function that clears this sliding panel. It is async because first it must clear child panels (cascade style) and once they are cleared this one can slide out and them finally return. Await this function to remove the panel and then do something once its gone.
  /// </summary>
  /// <returns>nothing! It's async</returns>
  public async Task RemoveScene() {
    if (!_isStable) {
      return;
    }

    if (_target is null || !IsInstanceValid(_target)) {
      return;
    }

    this.GetSafe(_pathSubSlidingRoot, out Control subSliders, false);
    if (subSliders is not null) {
      foreach (var c in subSliders.GetChildren()) {
        var c_comp = c.GetComponent<SlidingPanelComponent>();
        if (c_comp is null) {
          continue;
        }

        await c_comp.RemoveScene();
      }
    }

    _isStable = false;
    var tween = CreateTween().SetTrans(_transHide).SetEase(_easingHide);
    tween.TweenProperty(_target, "position:x", _target.Position.X - _target.Size.X, _popOutDuration);
    tween.TweenCallback(Callable.From(_target.QueueFree));
  }


}
