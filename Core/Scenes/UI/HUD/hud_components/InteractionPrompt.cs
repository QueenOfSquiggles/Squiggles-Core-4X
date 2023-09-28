namespace Squiggles.Core.Scenes.UI.HUD;

using Godot;
using Squiggles.Core.Events;
using Squiggles.Core.Extension;

public partial class InteractionPrompt : Label {

  [Export] private float _durationShow = 0.3f;
  [Export] private float _durationHide = 0.1f;

  private Tween _tween;

  public override void _Ready() {
    Text = "";
    EventBus.GUI.MarkAbleToInteract += OnAbleToInteract;
    EventBus.GUI.MarkUnableToInteract += OnUnableToInteract;
  }

  public override void _ExitTree() {
    EventBus.GUI.MarkAbleToInteract -= OnAbleToInteract;
    EventBus.GUI.MarkUnableToInteract -= OnUnableToInteract;
  }

  private void OnAbleToInteract(string text) {
    _tween?.Kill();
    _tween = GetTree().CreateTween().SetSC4XStyle();
    _tween.SetTrans(Tween.TransitionType.Bounce);
    VisibleRatio = 0.0f;
    Text = text;
    _tween.TweenProperty(this, "visible_ratio", 1.0f, _durationShow);
  }

  private void OnUnableToInteract() {
    _tween?.Kill();
    _tween = GetTree().CreateTween().SetSC4XStyle();
    _tween.SetTrans(Tween.TransitionType.Bounce);
    _tween.TweenProperty(this, "visible_ratio", 0.0f, _durationHide);
  }
}
