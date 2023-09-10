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
    [Export] private float TransitionTime = 0.2f;

    [ExportGroup("Inventory Stuff")]
    [Export] private PackedScene InventorySlotPacked;
    [ExportGroup("Player Money", "PlayerMoney")]
    [Export] private Color PlayerMoneyIncreaseCol = Colors.Lime;
    [Export] private Color PlayerMoneyDecreaseCol = Colors.Red;

    [ExportGroup("Paths", "Path")]
    [Export] private NodePath PathLabelSubtitle;
    [Export] private NodePath PathLabelAlert;

    [Export] private NodePath PathRootSubtitle;
    [Export] private NodePath PathRootAlert;
    [Export] private NodePath PathReticle;
    [Export] private NodePath PathInteractionPrompt;
    [Export] private NodePath PathGenericGuiRoot;
    [Export] private NodePath PathPlayerStatsHealthBar;
    [Export] private NodePath PathPlayerStatsHealthLabel;
    [Export] private NodePath PathPlayerStatsEnergyBar;
    [Export] private NodePath PathPlayerStatsEnergyLabel;
    [Export] private NodePath PathPlayerInventory;
    [Export] private NodePath PathPlayerMoneyTexture;
    [Export] private NodePath PathPlayerMoneyLabel;
    [Export] private NodePath PathPlayerMoneyPopLabel;



    private Label _LblSubtitle;
    private Label _LblAlert;
    private Control _RootSubtitle;
    private Control _RootAlert;
    private TextureRect _Reticle;
    private Label _InteractionPrompt;
    private Control _GenericGUIRoot;
    private TextureProgressBar _PlayerStatsHealthBar;
    private Label _PlayerStatsHealthLabel;
    private TextureProgressBar _PlayerStatsEnergyBar;
    private Label _PlayerStatsEnergyLabel;
    private Control _PlayerInventory;

    private Control _PlayerMoneyTexture;
    private Label _PlayerMoneyLabel;
    private Label _PlayerMoneyPopLabel;


    private Color COLOUR_TRANSPARENT = Color.FromString("#FFFFFF00", Colors.White);
    private Color COLOUR_VISIBLE = Colors.White;
    private Tween _PromptTween;

    private int _PreviousSelectSlot = 0;
    private int _LastKnownPlayerMoney = 0;

    public override void _Ready()
    {
        this.GetNode(PathLabelSubtitle, out _LblSubtitle);
        this.GetNode(PathLabelAlert, out _LblAlert);
        this.GetNode(PathRootSubtitle, out _RootSubtitle);
        this.GetNode(PathRootAlert, out _RootAlert);
        this.GetNode(PathReticle, out _Reticle);
        this.GetNode(PathInteractionPrompt, out _InteractionPrompt);
        this.GetSafe(PathGenericGuiRoot, out _GenericGUIRoot);
        this.GetSafe(PathPlayerStatsHealthBar, out _PlayerStatsHealthBar);
        this.GetSafe(PathPlayerStatsHealthLabel, out _PlayerStatsHealthLabel);
        this.GetSafe(PathPlayerStatsEnergyBar, out _PlayerStatsEnergyBar);
        this.GetSafe(PathPlayerStatsEnergyLabel, out _PlayerStatsEnergyLabel);
        this.GetSafe(PathPlayerInventory, out _PlayerInventory);
        this.GetSafe(PathPlayerMoneyTexture, out _PlayerMoneyTexture);
        this.GetSafe(PathPlayerMoneyLabel, out _PlayerMoneyLabel);
        this.GetSafe(PathPlayerMoneyPopLabel, out _PlayerMoneyPopLabel);

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
        _LblSubtitle.Text = text;
        HandleAnimation(_RootSubtitle, text.Length > 0);
    }

    public void ShowAlert(string text)
    {
        _LblAlert.Text = text;
        HandleAnimation(_RootAlert, text.Length > 0);
    }

    private void HandleAnimation(Control control, bool isVisible)
    {
        var tween = GetTree().CreateTween().SetDefaultStyle();
        var colour = isVisible ? COLOUR_VISIBLE : COLOUR_TRANSPARENT;
        tween.TweenProperty(control, "modulate", colour, 0.2f);
    }

    private void OnCanInteract(string text)
    {
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
        _GenericGUIRoot.RemoveAllChildren();
        _GenericGUIRoot.AddChild(gui);
    }

    private void OnRequestCloseGUI()
    {
        _GenericGUIRoot.RemoveAllChildren();
    }

    private void OnPlayerStatsUpdated(float health, float max_health, float energy, float max_energy)
    {
        var health_percent = health / max_health;
        var energy_percent = energy / max_energy;
        _PlayerStatsHealthBar.Value = health_percent;
        _PlayerStatsEnergyBar.Value = energy_percent;
        _PlayerStatsHealthLabel.Text = health.ToString("0");
        _PlayerStatsEnergyLabel.Text = energy.ToString("0");
    }

    private void OnInventorySlotUpdate(int index, string item, int qty)
    {
        if (_PlayerInventory.GetChildCount() <= index || index < 0) return;
        (_PlayerInventory.GetChild(index) as ItemSlotDisplay)?.UpdateItem(item, qty);
    }

    private void OnInventorySelect(int index)
    {
        if (_PlayerInventory.GetChildCount() <= index || index < 0) return;
        (_PlayerInventory.GetChild(_PreviousSelectSlot) as ItemSlotDisplay)?.OnDeselect();
        (_PlayerInventory.GetChild(index) as ItemSlotDisplay)?.OnSelect();
        _PreviousSelectSlot = index;
    }

    private void EnsureInventorySlots(int index)
    {
        while (index > _PlayerInventory.GetChildCount())
        {
            var slot = InventorySlotPacked.Instantiate();
            _PlayerInventory.AddChild(slot);
        }
    }

    private Tween _CurMoneyPopLabelTween;
    private const string _MoneyFormatString = "N0";
    private void OnPlayerMoneyChange(int new_total)
    {
        var delta = new_total - _LastKnownPlayerMoney;
        _LastKnownPlayerMoney = new_total;
        _PlayerMoneyLabel.Text = new_total.ToString(_MoneyFormatString);
        _PlayerMoneyPopLabel.Text = delta.ToString(_MoneyFormatString);
        _PlayerMoneyPopLabel.GlobalPosition = _PlayerMoneyLabel.GlobalPosition;
        _PlayerMoneyPopLabel.Scale = new Vector2(1.5f, 1.5f);
        _PlayerMoneyPopLabel.Visible = true;
        _PlayerMoneyPopLabel.Modulate = delta >= 0 ? PlayerMoneyIncreaseCol : PlayerMoneyDecreaseCol;

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
