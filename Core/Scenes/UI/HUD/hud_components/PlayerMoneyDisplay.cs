namespace Squiggles.Core.Scenes.UI.HUD;

using Godot;
using Squiggles.Core.Events;
using Squiggles.Core.Extension;

public partial class PlayerMoneyDisplay : TextureRect {


  private int _cacheVal;

  [ExportGroup("Pop Label")]
  [Export] private Color _increaseCol = Colors.Lime;
  [Export] private Color _decreaseCol = Colors.Red;
  [Export] private string _formatString = "N0";
  [Export] private float _popLabelYOffset = -64.0f;
  [Export] private float _popLabelDuration = 8.2f;

  [ExportGroup("Node Refs")]
  [Export] private Label _label;
  [Export] private Label _playerMoneyPopLabel;

  private Tween _tween;

  public override void _Ready()
    => EventBus.Gameplay.PlayerMoneyChanged += OnPlayerChange;
  public override void _ExitTree()
    => EventBus.Gameplay.PlayerMoneyChanged -= OnPlayerChange;

  private void OnPlayerChange(int n_val) {
    var delta = n_val - _cacheVal;
    _cacheVal = n_val;

    _label.Text = n_val.ToString(_formatString);
    _playerMoneyPopLabel.Text = delta.ToString(_formatString);
    _playerMoneyPopLabel.GlobalPosition = _label.GlobalPosition;
    _playerMoneyPopLabel.Scale = new Vector2(1.5f, 1.5f);
    _playerMoneyPopLabel.Visible = true;
    _playerMoneyPopLabel.Modulate = delta >= 0 ? _increaseCol : _decreaseCol;

    _tween?.Kill();
    _tween = CreateTween().SetSC4XStyle();
    _tween.TweenProperty(_playerMoneyPopLabel, "position:y", _playerMoneyPopLabel.Position.Y - _popLabelYOffset, _popLabelDuration);
    _tween.Parallel().TweenProperty(_playerMoneyPopLabel, "modulate:a", 0.0f, _popLabelDuration);
    _tween.Parallel().TweenProperty(_playerMoneyPopLabel, "scale", Vector2.One, _popLabelDuration);
    _tween.TweenCallback(Callable.From(() => {
      // cleanup
      _playerMoneyPopLabel.Text = "";
      _playerMoneyPopLabel.Position = _label.Position;
      _playerMoneyPopLabel.Scale = new Vector2(1.5f, 1.5f);
      _playerMoneyPopLabel.Visible = true;
    }));
  }

}
