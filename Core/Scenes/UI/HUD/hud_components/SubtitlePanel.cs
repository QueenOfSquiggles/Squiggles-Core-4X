namespace Squiggles.Core.Scenes.UI.HUD;

using Godot;
using Squiggles.Core.Events;

public partial class SubtitlePanel : HBoxContainer {

  [Export] private float _fadeDuration = 0.2f;
  [Export] private Label _subtitleLabel;

  private Tween _tween;
  public override void _Ready() {
    _subtitleLabel.Text = "";
    EventBus.GUI.RequestSubtitle += ShowSubtitle;
  }

  public override void _ExitTree()
    => EventBus.GUI.RequestSubtitle -= ShowSubtitle;

  private void ShowSubtitle(string text) {
    _subtitleLabel.Text = text;
    var target_colour = text.Length > 0 ? Colors.White : Colors.Transparent;
    _tween?.Kill();
    _tween.TweenProperty(this, "modulate", target_colour, _fadeDuration);
  }


}
