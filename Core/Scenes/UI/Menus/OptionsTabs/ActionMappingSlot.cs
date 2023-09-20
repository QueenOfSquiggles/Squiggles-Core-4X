namespace Squiggles.Core.Scenes.UI.Menus;

using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Extension;

public partial class ActionMappingSlot : HBoxContainer {

  [Signal] public delegate void ListenForActionEventHandler(string action_name);

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
    _actionButton.Text = Controls.Instance.GetCurrentMappingFor(TargetAction);
    Controls.Instance.OnControlMappingChanged += HandleMappingChanged;
  }

  public override void _ExitTree() => Controls.Instance.OnControlMappingChanged -= HandleMappingChanged;

  private void HandleMappingChanged(string action) {
    if (action != TargetAction || _actionButton is null) {
      return;
    }

    _actionButton.Text = Controls.Instance.GetCurrentMappingFor(TargetAction);
  }

  public void AssignButtonPressed() => EmitSignal(nameof(ListenForAction), TargetAction);
  public void ResetAction() => Controls.Instance.ResetMapping(TargetAction);
}
