namespace Squiggles.Core.Scenes.UI;

using Godot;
using Squiggles.Core.Attributes;
using Squiggles.Core.Data;
using Squiggles.Core.Events;
using Squiggles.Core.Extension;

/// <summary>
/// A fairly robust Heads Up Display (HUD) that handles many of the built-in signals such as subtitles, alerts, reticle, interaction prompts, and such.
/// </summary>
/// <remarks>
/// Future refactor should probably strip logic into the individual components such that the "DefaultHUD" scene is just a composition of independantly functioning elements. This would allow users to quickly get up and running with a default hud, then customize as they develop.
/// </remarks>
[MarkForRefactor("Separation of Duties", "This class manages all of the individual, independant components that can make up a HUD. Splitting responsibilities would allow more customization for HUD layout")]
public partial class DefaultHUD : Control {

  /// <summary>
  /// The time it takes for the reticle to transition between scales.
  /// </summary>
  [ExportGroup("Reticle Settings")]
  [Export] private float _transitionTime = 0.2f;

  /// <summary>
  /// The packed scene for the inventory
  /// </summary>
  [ExportGroup("Inventory Stuff")]
  [Export] private PackedScene _inventorySlotPacked;

  /// <summary>
  /// The colour for the player's money pop label. Specifically when money is increasing
  /// </summary>
  [ExportGroup("Player Money", "_playerMoney")]
  [Export] private Color _playerMoneyIncreaseCol = Colors.Lime;

  /// <summary>
  /// The colour for the player's money pop label. Specifically when money is decreasing
  /// </summary>
  [Export] private Color _playerMoneyDecreaseCol = Colors.Red;

  /// <summary>
  /// the label that takes in subtitle text
  /// </summary>
  [ExportGroup("Paths")]
  [Export] private Label _lblSubtitle;
  /// <summary>
  /// The label for taking in alert text
  /// </summary>
  [Export] private Label _lblAlert;
  /// <summary>
  /// The root of the subtitle panel
  /// </summary>
  [Export] private Control _rootSubtitle;
  /// <summary>
  /// The root of the alert panel
  /// </summary>
  [Export] private Control _rootAlert;
  /// <summary>
  /// The reticle texture
  /// </summary>
  [Export] private TextureRect _reticle;
  /// <summary>
  /// The label which handles interation prompts
  /// </summary>
  [Export] private Label _interactionPrompt;
  /// <summary>
  /// The root control to load generic GUI elements into
  /// </summary>
  [Export] private Control _genericGUIRoot;
  /// <summary>
  /// The progress bar related to the player's health
  /// </summary>
  [Export] private TextureProgressBar _playerStatsHealthBar;
  /// <summary>
  /// The label for the player health
  /// </summary>
  [Export] private Label _playerStatsHealthLabel;
  /// <summary>
  /// the progress bar for the player's energy
  /// </summary>
  [Export] private TextureProgressBar _playerStatsEnergyBar;
  /// <summary>
  /// the label for the player's energy
  /// </summary>
  [Export] private Label _playerStatsEnergyLabel;
  /// <summary>
  /// the root control for the player's inventory to be instanced in
  /// </summary>
  [Export] private Control _playerInventory;
  /// <summary>
  /// The texture for the money icon
  /// </summary>
  [Export] private Control _playerMoneyTexture;
  /// <summary>
  /// the label for the money display
  /// </summary>
  [Export] private Label _playerMoneyLabel;
  /// <summary>
  /// the label that is used as a pop label using <see cref="SceneTreeTween"/>
  /// </summary>
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

    if (SC4X.Config?.EnableReticle is not true) {
      _reticle.QueueFree();
      _reticle = null;
    }
    else {
      _reticle.Scale = Vector2.One * Access.ReticleHiddenScale;
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

    var tween = GetTree().CreateTween().SetSC4XStyle();
    var colour = isVisible ? _colourVisible : _colourTransparent;
    tween.TweenProperty(control, "modulate", colour, 0.2f);
  }

  private void OnCanInteract(string text) {
    if (_interactionPrompt is null) {
      return;
    }

    _promptTween?.Kill();
    _promptTween = GetTree().CreateTween().SetSC4XStyle();
    _promptTween.SetTrans(Tween.TransitionType.Bounce);
    _interactionPrompt.VisibleRatio = 0.0f;
    _interactionPrompt.Text = text;
    if (_reticle is not null) {
      _promptTween.TweenProperty(_reticle, "scale", Vector2.One * Access.ReticleShownScale, 0.3f);
    }
    _promptTween.TweenProperty(_interactionPrompt, "visible_ratio", 1.0f, 0.3f);
  }

  private void OnCannotInteract() {
    _promptTween?.Kill();
    _promptTween = GetTree().CreateTween().SetSC4XStyle();
    _promptTween.SetTrans(Tween.TransitionType.Bounce);

    if (_reticle is not null) {
      _promptTween.TweenProperty(_reticle, "scale", Vector2.One * Access.ReticleHiddenScale, 0.3f);
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

  /// <summary>
  /// A functionality I would like to preserve is to allow hiding the HUD with F1. Not only is it incredibly useful for making cinematics for trailers and videos, but it's generally a nice feature for players as well (think minecraft). Making your game easier for youtubers/streamers to play makes it easier to get your game into public consciousness (spelling?) and that's pretty fricking awesome if it happens.
  /// </summary>
  public override void _Input(InputEvent @event) {
    if (@event is InputEventKey kp && kp.Keycode == Key.F1 && kp.IsPressed()) {
      Visible = !Visible; // toggle visibility of HUD for cinematics or other useful things
    }
  }

}
