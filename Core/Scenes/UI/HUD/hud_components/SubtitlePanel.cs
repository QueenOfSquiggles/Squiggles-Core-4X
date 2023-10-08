namespace Squiggles.Core.Scenes.UI.HUD;

using Godot;
using Squiggles.Core.Events;
using Squiggles.Core.Extension;

public partial class SubtitlePanel : HBoxContainer {

  [Export] private float _fadeDuration = 0.2f;
  [Export] private Label _subtitleLabel;

  private Tween _tween;
  public override void _Ready() {
    var tempFade = _fadeDuration;
    _fadeDuration = 0.01f;
    ShowSubtitle("");
    Visible = false;

    EventBus.GUI.RequestSubtitle += ShowSubtitle;
  }

  public override void _ExitTree()
    => EventBus.GUI.RequestSubtitle -= ShowSubtitle;

  private void ShowSubtitle(string text) {
    _subtitleLabel.Text = text;
    Visible = text.Length > 0;
    var target_colour = Visible ? Colors.White : Colors.Transparent;
    _tween?.Kill();
    _tween = GetTree().CreateTween().SetSC4XStyle();
    _tween.TweenProperty(this, "modulate", target_colour, _fadeDuration);
  }


}
