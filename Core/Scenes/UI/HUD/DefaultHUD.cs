using System;
using System.Threading.Tasks;
using Godot;
using queen.data;
using queen.error;
using queen.events;
using queen.extension;

public partial class DefaultHUD : Control
{

    [ExportGroup("Reticle Settings")]
    [Export] private float _TransitionTime = 0.2f;

    [ExportGroup("Inventory Stuff")]
    [Export] private PackedScene _InventorySlotPacked;

    [ExportGroup("Player Money", "PlayerMoney")]
    [Export] private Color _PlayerMoneyIncreaseCol = Colors.Lime;
    [Export] private Color _PlayerMoneyDecreaseCol = Colors.Red;

    [ExportGroup("Paths", "Path")]
    [Export] private Label _LblSubtitle;
    [Export] private Label _LblAlert;
    [Export] private Control _RootSubtitle;
    [Export] private Control _RootAlert;
    [Export] private TextureRect _Reticle;
    [Export] private Label _InteractionPrompt;
    [Export] private Control _GenericGUIRoot;
    [Export] private TextureProgressBar _PlayerStatsHealthBar;
    [Export] private Label _PlayerStatsHealthLabel;
    [Export] private TextureProgressBar _PlayerStatsEnergyBar;
    [Export] private Label _PlayerStatsEnergyLabel;
    [Export] private Control _PlayerInventory;
    [Export] private Control _PlayerMoneyTexture;
    [Export] private Label _PlayerMoneyLabel;
    [Export] private Label _PlayerMoneyPopLabel;


    private Color COLOUR_TRANSPARENT = Color.FromString("#FFFFFF00", Colors.White);
    private Color COLOUR_VISIBLE = Colors.White;
    private Tween _PromptTween;

    private int _PreviousSelectSlot = 0;
    private int _LastKnownPlayerMoney = 0;

    public override void _Ready()
    {
        if (_LblSubtitle is null) return;
        if (_LblAlert is null) return;
        if (_RootSubtitle is null) return;
        if (_RootAlert is null) return;
        if (_LblSubtitle is null) return;
        if (_Reticle is null) return;
        if (_InteractionPrompt is null) return;
        if (_PlayerMoneyLabel is null) return;
        if (_PlayerMoneyPopLabel is null) return;

        _LblSubtitle.Text = "";
        _LblAlert.Text = "";


        _RootSubtitle.Modulate = COLOUR_TRANSPARENT;
        _RootAlert.Modulate = COLOUR_TRANSPARENT;

        _Reticle.Scale = Vector2.One * Access.Instance.ReticleHiddenScale;
        _InteractionPrompt.Text = "";

        _PlayerMoneyLabel.Text = "";
        _PlayerMoneyPopLabel.Text = "";
        _PlayerMoneyPopLabel.Position = _PlayerMoneyLabel.Position;
        _PlayerMoneyPopLabel.Scale = new Vector2(1.5f, 1.5f);
        _PlayerMoneyPopLabel.Visible = false;



        Events.GUI.RequestSubtitle += ShowSubtitle;
        Events.GUI.RequestAlert += ShowAlert;
        Events.GUI.MarkAbleToInteract += OnCanInteract;
        Events.GUI.MarkUnableToInteract += OnCannotInteract;
        Events.GUI.RequestGUI += OnRequestGenericGUI;
        Events.GUI.RequestCloseGUI += OnRequestCloseGUI;
        Events.Gameplay.OnPlayerStatsUpdated += OnPlayerStatsUpdated;
        Events.GUI.UpdatePlayerInventoryDisplay += OnInventorySlotUpdate;
        Events.GUI.PlayerInventorySelectIndex += OnInventorySelect;
        Events.GUI.PlayerInventorySizeChange += EnsureInventorySlots;
        Events.Gameplay.PlayerMoneyChanged += OnPlayerMoneyChange;
    }

    public override void _ExitTree()
    {
        Events.GUI.RequestSubtitle -= ShowSubtitle;
        Events.GUI.RequestAlert -= ShowAlert;
        Events.GUI.MarkAbleToInteract -= OnCanInteract;
        Events.GUI.MarkUnableToInteract -= OnCannotInteract;
        Events.GUI.RequestGUI -= OnRequestGenericGUI;
        Events.GUI.RequestCloseGUI -= OnRequestCloseGUI;
        Events.Gameplay.OnPlayerStatsUpdated -= OnPlayerStatsUpdated;
        Events.GUI.UpdatePlayerInventoryDisplay -= OnInventorySlotUpdate;
        Events.GUI.UpdatePlayerInventoryDisplay -= OnInventorySlotUpdate;
        Events.GUI.PlayerInventorySelectIndex -= OnInventorySelect;
        Events.GUI.PlayerInventorySizeChange -= EnsureInventorySlots;
        Events.Gameplay.PlayerMoneyChanged -= OnPlayerMoneyChange;

    }

    public void ShowSubtitle(string text)
    {
        if (_LblSubtitle is null) return;
        _LblSubtitle.Text = text;
        HandleAnimation(_RootSubtitle, text.Length > 0);
    }

    public void ShowAlert(string text)
    {
        if (_LblAlert is null) return;
        _LblAlert.Text = text;
        HandleAnimation(_RootAlert, text.Length > 0);
    }

    private void HandleAnimation(Control control, bool isVisible)
    {
        if (control is null) return;
        var tween = GetTree().CreateTween().SetDefaultStyle();
        var colour = isVisible ? COLOUR_VISIBLE : COLOUR_TRANSPARENT;
        tween.TweenProperty(control, "modulate", colour, 0.2f);
    }

    private void OnCanInteract(string text)
    {
        if (_InteractionPrompt is null) return;
        _PromptTween?.Kill();
        _PromptTween = GetTree().CreateTween().SetDefaultStyle();
        _PromptTween.SetTrans(Tween.TransitionType.Bounce);
        _InteractionPrompt.VisibleRatio = 0.0f;
        _InteractionPrompt.Text = text;

        _PromptTween.TweenProperty(_Reticle, "scale", Vector2.One * Access.Instance.ReticleShownScale, 0.3f);
        _PromptTween.TweenProperty(_InteractionPrompt, "visible_ratio", 1.0f, 0.3f);
    }

    private void OnCannotInteract()
    {
        _PromptTween?.Kill();
        _PromptTween = GetTree().CreateTween().SetDefaultStyle();
        _PromptTween.SetTrans(Tween.TransitionType.Bounce);

        _PromptTween.TweenProperty(_Reticle, "scale", Vector2.One * Access.Instance.ReticleHiddenScale, 0.3f);
        _PromptTween.TweenProperty(_InteractionPrompt, "visible_ratio", 0.0f, 0.1f);
    }

    private void OnRequestGenericGUI(Control gui)
    {
        _GenericGUIRoot?.RemoveAllChildren();
        _GenericGUIRoot?.AddChild(gui);
    }

    private void OnRequestCloseGUI()
    {
        _GenericGUIRoot?.RemoveAllChildren();
    }

    private void OnPlayerStatsUpdated(float health, float max_health, float energy, float max_energy)
    {
        if (_PlayerStatsHealthBar is null) return;
        if (_PlayerStatsEnergyBar is null) return;
        if (_PlayerStatsHealthLabel is null) return;
        if (_PlayerStatsEnergyLabel is null) return;

        var health_percent = health / max_health;
        var energy_percent = energy / max_energy;
        _PlayerStatsHealthBar.Value = health_percent;
        _PlayerStatsEnergyBar.Value = energy_percent;
        _PlayerStatsHealthLabel.Text = health.ToString("0");
        _PlayerStatsEnergyLabel.Text = energy.ToString("0");
    }

    private void OnInventorySlotUpdate(int index, string item, int qty)
    {
        if (_PlayerInventory is null) return;
        if (_PlayerInventory.GetChildCount() <= index || index < 0) return;
        (_PlayerInventory.GetChild(index) as ItemSlotDisplay)?.UpdateItem(item, qty);
    }

    private void OnInventorySelect(int index)
    {
        if (_PlayerInventory is null) return;
        if (_PlayerInventory.GetChildCount() <= index || index < 0) return;
        (_PlayerInventory.GetChild(_PreviousSelectSlot) as ItemSlotDisplay)?.OnDeselect();
        (_PlayerInventory.GetChild(index) as ItemSlotDisplay)?.OnSelect();
        _PreviousSelectSlot = index;
    }

    private void EnsureInventorySlots(int index)
    {
        if (_PlayerInventory is null) return;
        if (_InventorySlotPacked is null) return;

        while (index > _PlayerInventory.GetChildCount())
        {
            var slot = _InventorySlotPacked.Instantiate();
            _PlayerInventory.AddChild(slot);
        }
    }

    private Tween _CurMoneyPopLabelTween;
    private const string _MoneyFormatString = "N0";
    private void OnPlayerMoneyChange(int new_total)
    {
        if (_PlayerMoneyLabel is null) return;
        if (_PlayerMoneyPopLabel is null) return;
        if (_PlayerMoneyTexture is null) return;

        var delta = new_total - _LastKnownPlayerMoney;
        _LastKnownPlayerMoney = new_total;
        _PlayerMoneyLabel.Text = new_total.ToString(_MoneyFormatString);
        _PlayerMoneyPopLabel.Text = delta.ToString(_MoneyFormatString);
        _PlayerMoneyPopLabel.GlobalPosition = _PlayerMoneyLabel.GlobalPosition;
        _PlayerMoneyPopLabel.Scale = new Vector2(1.5f, 1.5f);
        _PlayerMoneyPopLabel.Visible = true;
        _PlayerMoneyPopLabel.Modulate = delta >= 0 ? _PlayerMoneyIncreaseCol : _PlayerMoneyDecreaseCol;

        _CurMoneyPopLabelTween?.Kill();
        _CurMoneyPopLabelTween = _PlayerMoneyTexture.CreateTween().SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
        _CurMoneyPopLabelTween.TweenProperty(_PlayerMoneyPopLabel, "position:y", _PlayerMoneyPopLabel.Position.Y - 64.0f, 0.5f);
        _CurMoneyPopLabelTween.Parallel().TweenProperty(_PlayerMoneyPopLabel, "modulate:a", 0.0f, 0.7f);
        _CurMoneyPopLabelTween.Parallel().TweenProperty(_PlayerMoneyPopLabel, "scale", Vector2.One, 0.7f);
        _CurMoneyPopLabelTween.TweenCallback(Callable.From(() =>
        {
            // cleanup
            _PlayerMoneyPopLabel.Text = "";
            _PlayerMoneyPopLabel.Position = _PlayerMoneyLabel.Position;
            _PlayerMoneyPopLabel.Scale = new Vector2(1.5f, 1.5f);
            _PlayerMoneyPopLabel.Visible = true;
        }));
    }


    public override void _Input(InputEvent e)
    {
        if (e is InputEventKey kp && kp.Keycode == Key.F1 && kp.IsPressed())
        {
            Visible = !Visible; // toggle visibility of HUD for cinematics or other useful things
        }
    }

}
