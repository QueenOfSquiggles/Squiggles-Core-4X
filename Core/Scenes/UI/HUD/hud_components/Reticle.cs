namespace Squiggles.Core.Scenes.UI.HUD;

using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Events;
using Squiggles.Core.Extension;

public partial class Reticle : TextureRect {

  [Export] private float _transitionTime = 0.3f;
  private Tween _tween;

  public override void _Ready() {
    if (SC4X.Config?.EnableReticle is not true) {
      QueueFree();
      return;
    }
    Scale = Vector2.One * Access.ReticleHiddenScale;
    EventBus.GUI.MarkAbleToInteract += OnCanInteract;
    EventBus.GUI.MarkUnableToInteract += OnCannotInteract;
  }

  public override void _ExitTree() {
    EventBus.GUI.MarkAbleToInteract -= OnCanInteract;
    EventBus.GUI.MarkUnableToInteract -= OnCannotInteract;
  }

  private void OnCanInteract(string _) {
    _tween?.Kill();
    _tween = GetTree().CreateTween().SetSC4XStyle();
    _tween.TweenProperty(this, "scale", Vector2.One * Access.ReticleShownScale, _transitionTime);
  }
  private void OnCannotInteract() {
    _tween?.Kill();
    _tween = GetTree().CreateTween().SetSC4XStyle();
    _tween.TweenProperty(this, "scale", Vector2.One * Access.ReticleHiddenScale, _transitionTime);
  }

}
