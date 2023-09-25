namespace Squiggles.Core.Scenes.UI;

using Godot;
using Squiggles.Core.Attributes;
using Squiggles.Core.Scenes.Registration;
using Squiggles.Core.WorldEntity;
using static Godot.Tween;

/// <summary>
/// A template for displaying a single inventory slot. Misguided and badly implemented. Skipping documentation for this since it's intended to be scrapped/drastically changed
/// </summary>
[MarkForRefactor("Remove/Improve", "Inventories are something highly specific to individual games. Maybe refactor into a number of templates?")]
public partial class ItemSlotDisplay : PanelContainer {
  [ExportGroup("Selected Settings", "_Select")]
  [Export] private float _selectOffset = -16.0f;
  [Export] private float _unselectedOffset = 32.0f;
  [Export] private float _selectScale = 1.25f;

  [ExportGroup("Node Paths")]
  [Export] private Label _label;
  [Export] private TextureRect _icon;

  private string _item = "";
  private int _qty;
  private Tween _activeTween;
  private float _returnScale = 1.0f;
  private float _selectY;
  private float _unselectY;

  public override void _Ready() {
    PivotOffset = new Vector2(Size.X / 2.0f, Size.Y);
    _selectY = Position.Y + _selectOffset;
    _unselectY = Position.Y + _unselectedOffset;
    Position += new Vector2(0, Size.Y);
    if (_label is not null) {
      _label.Text = "";
    }

    UpdateItem("", 0);

    _activeTween = GetDefaultTween();
    _activeTween.TweenInterval(0.5f);
    _activeTween.TweenProperty(this, "position:y", _unselectY, 0.3f);
  }

  public void UpdateItem(string item, int qty) {
    if (_icon is null || _label is null) {
      return;
    }

    var pos = Position;
    var scale = Scale;
    if (item == "") {
      _item = "";
      _qty = 0;
      _icon.Texture = null;
      _label.Text = "";
    }
    if (item != _item) {
      _item = item;
      var data = RegistrationManager.GetResource<WorldEntity>(_item);
      _icon.Texture = data?.InventoryIcon as Texture2D;
    }

    if (_qty != qty) {
      _qty = qty;
      _label.Text = (_qty <= 1) ? "" : $"{_qty}";
    }
    Position = pos;
    Scale = scale;
  }

  public void OnSelect() {
    _activeTween?.Kill();
    _activeTween = GetDefaultTween();
    _activeTween.TweenProperty(this, "position:y", _selectY, 0.3f);
    _activeTween.Parallel().TweenProperty(this, "scale", Vector2.One * _selectScale, 0.3f);
    _activeTween.Parallel().TweenProperty(this, "z_index", 3, 0.3f);
  }

  public void OnDeselect() {
    _activeTween?.Kill();
    _activeTween = GetDefaultTween();
    _activeTween.TweenProperty(this, "position:y", _unselectY, 0.3f);
    _activeTween.Parallel().TweenProperty(this, "scale", Vector2.One * _returnScale, 0.3f);
    _activeTween.Parallel().TweenProperty(this, "z_index", 0, 0.3f);
  }

  private Tween GetDefaultTween() => CreateTween().SetTrans(TransitionType.Cubic).SetEase(EaseType.InOut);


}
