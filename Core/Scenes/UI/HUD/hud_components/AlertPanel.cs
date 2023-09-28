namespace Squiggles.Core.Scenes.UI.HUD;

using Godot;
using Squiggles.Core.Events;
using Squiggles.Core.Extension;

public partial class AlertPanel : PanelContainer {

  [Export] private float _fadeDuration = 0.7f;
  [Export] private Label _label;

  private Tween _tween;

  public override void _Ready() {
    _label.Text = "";
    EventBus.GUI.RequestAlert += ShowAlert;
  }

  public override void _ExitTree() => EventBus.GUI.RequestAlert -= ShowAlert;


  private void ShowAlert(string alertText) {

    if (alertText.Length > 0) {
      // showing a new alert
      _label.Text = alertText;
      _tween?.Kill();
      _tween = GetTree().CreateTween().SetSC4XStyle();
      _tween.TweenProperty(this, "modulate", Colors.White, _fadeDuration);
    }
    else {
      // hiding a current alert
      _tween?.Kill();
      _tween = GetTree().CreateTween().SetSC4XStyle();
      _tween.TweenProperty(this, "modulate", Colors.Transparent, _fadeDuration);
      _tween.TweenCallback(Callable.From(() => _label.Text = alertText));
    }
  }


}

