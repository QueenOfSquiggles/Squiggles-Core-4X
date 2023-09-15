namespace Squiggles.Core.Scenes.UI;

using System.Threading.Tasks;
using Godot;
using Squiggles.Core.Extension;

[GlobalClass]
public partial class SlidingPanelComponent : Node {

  [Export] private float _popInDuration = 0.3f;
  [Export] private float _popOutDuration = 0.3f;
  [Export] private Control _target;
  [Export] private NodePath _pathSubSlidingRoot = "..";
  [Export] private Tween.TransitionType _transShow = Tween.TransitionType.Cubic;
  [Export] private Tween.TransitionType _transHide = Tween.TransitionType.Cubic;
  [Export] private Tween.EaseType _easingShow = Tween.EaseType.Out;
  [Export] private Tween.EaseType _easingHide = Tween.EaseType.Out;

  private bool _isStable; // mood

  public override void _Ready() {
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

  public async Task RemoveScene() {
    if (!_isStable) {
      return;
    }

    if (_target is null) {
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
