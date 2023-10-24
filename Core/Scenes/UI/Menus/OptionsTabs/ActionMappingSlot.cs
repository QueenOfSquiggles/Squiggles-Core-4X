namespace Squiggles.Core.Scenes.UI.Menus;

using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Extension;

/// <summary>
/// A GUI element for handling an action mapping.
/// </summary>
public partial class ActionMappingSlot : HBoxContainer {

  /// <summary>
  /// A signal for when the remap button is pressed. Signals up to <see cref="ControlsTab"/>
  /// </summary>
  /// <param name="action_name">the name of the action managed by this slot: <see cref="TargetAction"/></param>
  [Signal] public delegate void ListenForActionEventHandler(string action_name);

  /// <summary>
  /// The action map key stored in this slot
  /// </summary>
  [Export] public string TargetAction = "";

  private Label _label;
  private Button _actionButton;
  public override void _Ready() {
    this.GetNode("Label", out _label);
    this.GetNode("BtnListen", out _actionButton);

    if (_label is null || _actionButton is null) {
      return;
    }

    _label.Text = TargetAction.Replace("_", " ");
    _actionButton.Text = Controls.GetCurrentMappingFor(TargetAction);
    Controls.OnControlMappingChanged += HandleMappingChanged;
  }

  public override void _ExitTree() => Controls.OnControlMappingChanged -= HandleMappingChanged;

  private void HandleMappingChanged(string action) {
    if (action != TargetAction || _actionButton is null) {
      return;
    }

    _actionButton.Text = Controls.GetCurrentMappingFor(TargetAction);
  }

  public void AssignButtonPressed() => EmitSignal(nameof(ListenForAction), TargetAction);
  public void ResetAction() => Controls.ResetMapping(TargetAction);
}
