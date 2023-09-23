namespace Squiggles.Core.Scenes.UI;

using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Events;
using Squiggles.Core.Extension;

public partial class DefaultHUD : Control {

  [ExportGroup("Reticle Settings")]
  [Export] private float _transitionTime = 0.2f;

  [ExportGroup("Inventory Stuff")]
  [Export] private PackedScene _inventorySlotPacked;

  [ExportGroup("Player Money", "PlayerMoney")]
  [Export] private Color _playerMoneyIncreaseCol = Colors.Lime;
  [Export] private Color _playerMoneyDecreaseCol = Colors.Red;

  [ExportGroup("Paths", "Path")]
  [Export] private Label _lblSubtitle;
  [Export] private Label _lblAlert;
  [Export] private Control _rootSubtitle;
  [Export] private Control _rootAlert;
  [Export] private TextureRect _reticle;
  [Export] private Label _interactionPrompt;
  [Export] private Control _genericGUIRoot;
  [Export] private TextureProgressBar _playerStatsHealthBar;
  [Export] private Label _playerStatsHealthLabel;
  [Export] private TextureProgressBar _playerStatsEnergyBar;
  [Export] private Label _playerStatsEnergyLabel;
  [Export] private Control _playerInventory;
  [Export] private Control _playerMoneyTexture;
  [Export] private Label _playerMoneyLabel;
  [Export] private Label _playerMoneyPopLabel;


  private Color _colourTransparent = Color.FromString("#FFFFFF00", Colors.White);
  private Color _colourVisible = Colors.White;
  private Tween _promptTween;

  private int _previousSelectSlot;
  private int _lastKnownPlayerMoney;

  public override void _Ready() {
    _lblSubtitle.Text = "";
    _lblAlert.Text = "";


    _rootSubtitle.Modulate = _colourTransparent;
    _rootAlert.Modulate = _colourTransparent;

    if (ThisIsYourMainScene.Config?.EnableReticle is not true) {
      _reticle.QueueFree();
      _reticle = null;
    }
    else {
      _reticle.Scale = Vector2.One * Access.Instance.ReticleHiddenScale;
    }

    _interactionPrompt.Text = "";

    _playerMoneyLabel.Text = "";
    _playerMoneyPopLabel.Text = "";
    _playerMoneyPopLabel.Position = _playerMoneyLabel.Position;
    _playerMoneyPopLabel.Scale = new Vector2(1.5f, 1.5f);
    _playerMoneyPopLabel.Visible = false;



    EventBus.GUI.RequestSubtitle += ShowSubtitle;
    EventBus.GUI.RequestAlert += ShowAlert;
    EventBus.GUI.MarkAbleToInteract += OnCanInteract;
    EventBus.GUI.MarkUnableToInteract += OnCannotInteract;
    EventBus.GUI.RequestGUI += OnRequestGenericGUI;
    EventBus.GUI.RequestCloseGUI += OnRequestCloseGUI;
    EventBus.Gameplay.OnPlayerStatsUpdated += OnPlayerStatsUpdated;
    EventBus.GUI.UpdatePlayerInventoryDisplay += OnInventorySlotUpdate;
    EventBus.GUI.PlayerInventorySelectIndex += OnInventorySelect;
    EventBus.GUI.PlayerInventorySizeChange += EnsureInventorySlots;
    EventBus.Gameplay.PlayerMoneyChanged += OnPlayerMoneyChange;
  }

  public override void _ExitTree() {
    EventBus.GUI.RequestSubtitle -= ShowSubtitle;
    EventBus.GUI.RequestAlert -= ShowAlert;
    EventBus.GUI.MarkAbleToInteract -= OnCanInteract;
    EventBus.GUI.MarkUnableToInteract -= OnCannotInteract;
    EventBus.GUI.RequestGUI -= OnRequestGenericGUI;
    EventBus.GUI.RequestCloseGUI -= OnRequestCloseGUI;
    EventBus.Gameplay.OnPlayerStatsUpdated -= OnPlayerStatsUpdated;
    EventBus.GUI.UpdatePlayerInventoryDisplay -= OnInventorySlotUpdate;
    EventBus.GUI.UpdatePlayerInventoryDisplay -= OnInventorySlotUpdate;
    EventBus.GUI.PlayerInventorySelectIndex -= OnInventorySelect;
    EventBus.GUI.PlayerInventorySizeChange -= EnsureInventorySlots;
    EventBus.Gameplay.PlayerMoneyChanged -= OnPlayerMoneyChange;

  }

  public void ShowSubtitle(string text) {
    if (_lblSubtitle is null) {
      return;
    }

    _lblSubtitle.Text = text;
    HandleAnimation(_rootSubtitle, text.Length > 0);
  }

  public void ShowAlert(string text) {
    if (_lblAlert is null) {
      return;
    }

    _lblAlert.Text = text;
    HandleAnimation(_rootAlert, text.Length > 0);
  }

  private void HandleAnimation(Control control, bool isVisible) {
    if (control is null) {
      return;
    }

    var tween = GetTree().CreateTween().SetDefaultStyle();
    var colour = isVisible ? _colourVisible : _colourTransparent;
    tween.TweenProperty(control, "modulate", colour, 0.2f);
  }

  private void OnCanInteract(string text) {
    if (_interactionPrompt is null) {
      return;
    }

    _promptTween?.Kill();
    _promptTween = GetTree().CreateTween().SetDefaultStyle();
    _promptTween.SetTrans(Tween.TransitionType.Bounce);
    _interactionPrompt.VisibleRatio = 0.0f;
    _interactionPrompt.Text = text;
    if (_reticle is not null) {
      _promptTween.TweenProperty(_reticle, "scale", Vector2.One * Access.Instance.ReticleShownScale, 0.3f);
    }
    _promptTween.TweenProperty(_interactionPrompt, "visible_ratio", 1.0f, 0.3f);
  }

  private void OnCannotInteract() {
    _promptTween?.Kill();
    _promptTween = GetTree().CreateTween().SetDefaultStyle();
    _promptTween.SetTrans(Tween.TransitionType.Bounce);

    if (_reticle is not null) {
      _promptTween.TweenProperty(_reticle, "scale", Vector2.One * Access.Instance.ReticleHiddenScale, 0.3f);
    }
    _promptTween.TweenProperty(_interactionPrompt, "visible_ratio", 0.0f, 0.1f);
  }

  private void OnRequestGenericGUI(Control gui) {
    _genericGUIRoot?.RemoveAllChildren();
    _genericGUIRoot?.AddChild(gui);
  }

  private void OnRequestCloseGUI() => _genericGUIRoot?.RemoveAllChildren();

  private void OnPlayerStatsUpdated(float health, float max_health, float energy, float max_energy) {
    var health_percent = health / max_health;
    var energy_percent = energy / max_energy;
    _playerStatsHealthBar.Value = health_percent;
    _playerStatsEnergyBar.Value = energy_percent;
    _playerStatsHealthLabel.Text = health.ToString("0");
    _playerStatsEnergyLabel.Text = energy.ToString("0");
  }

  private void OnInventorySlotUpdate(int index, string item, int qty) {
    if (_playerInventory is null || _playerInventory.GetChildCount() <= index || index < 0) {
      return;
    }
    (_playerInventory.GetChild(index) as ItemSlotDisplay)?.UpdateItem(item, qty);
  }

  private void OnInventorySelect(int index) {
    if (_playerInventory is null || _playerInventory.GetChildCount() <= index || index < 0) {
      return;
    }
    (_playerInventory.GetChild(_previousSelectSlot) as ItemSlotDisplay)?.OnDeselect();
    (_playerInventory.GetChild(index) as ItemSlotDisplay)?.OnSelect();
    _previousSelectSlot = index;
  }

  private void EnsureInventorySlots(int index) {
    while (index > _playerInventory.GetChildCount()) {
      var slot = _inventorySlotPacked.Instantiate();
      _playerInventory.AddChild(slot);
    }
  }

  private Tween _curMoneyPopLabelTween;
  private const string MONEY_FORMAT_STRING = "N0";
  private void OnPlayerMoneyChange(int new_total) {

    var delta = new_total - _lastKnownPlayerMoney;
    _lastKnownPlayerMoney = new_total;
    _playerMoneyLabel.Text = new_total.ToString(MONEY_FORMAT_STRING);
    _playerMoneyPopLabel.Text = delta.ToString(MONEY_FORMAT_STRING);
    _playerMoneyPopLabel.GlobalPosition = _playerMoneyLabel.GlobalPosition;
    _playerMoneyPopLabel.Scale = new Vector2(1.5f, 1.5f);
    _playerMoneyPopLabel.Visible = true;
    _playerMoneyPopLabel.Modulate = delta >= 0 ? _playerMoneyIncreaseCol : _playerMoneyDecreaseCol;

    _curMoneyPopLabelTween?.Kill();
    _curMoneyPopLabelTween = _playerMoneyTexture.CreateTween().SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
    _curMoneyPopLabelTween.TweenProperty(_playerMoneyPopLabel, "position:y", _playerMoneyPopLabel.Position.Y - 64.0f, 0.5f);
    _curMoneyPopLabelTween.Parallel().TweenProperty(_playerMoneyPopLabel, "modulate:a", 0.0f, 0.7f);
    _curMoneyPopLabelTween.Parallel().TweenProperty(_playerMoneyPopLabel, "scale", Vector2.One, 0.7f);
    _curMoneyPopLabelTween.TweenCallback(Callable.From(() => {
      // cleanup
      _playerMoneyPopLabel.Text = "";
      _playerMoneyPopLabel.Position = _playerMoneyLabel.Position;
      _playerMoneyPopLabel.Scale = new Vector2(1.5f, 1.5f);
      _playerMoneyPopLabel.Visible = true;
    }));
  }


  public override void _Input(InputEvent @event) {
    if (@event is InputEventKey kp && kp.Keycode == Key.F1 && kp.IsPressed()) {
      Visible = !Visible; // toggle visibility of HUD for cinematics or other useful things
    }
  }

}
