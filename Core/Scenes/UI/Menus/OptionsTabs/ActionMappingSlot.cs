using Godot;
using queen.data;
using queen.extension;
using System;

public partial class ActionMappingSlot : HBoxContainer
{

    [Signal] public delegate void ListenForActionEventHandler(string action_name);

    [Export] public string TargetAction = "";

    private Label _Label;
    private Button _ActionButton;
    public override void _Ready()
    {
        this.GetNode("Label", out _Label);
        this.GetNode("BtnListen", out _ActionButton);

        if (_Label is null || _ActionButton is null) return;

        _Label.Text = TargetAction.Replace("_", " ");
        _ActionButton.Text = Controls.Instance.GetCurrentMappingFor(TargetAction);
        Controls.Instance.OnControlMappingChanged += HandleMappingChanged;
    }

    public override void _ExitTree()
    {
        Controls.Instance.OnControlMappingChanged -= HandleMappingChanged;
    }

    private void HandleMappingChanged(string action)
    {
        if (action != TargetAction || _ActionButton is null) return;
        _ActionButton.Text = Controls.Instance.GetCurrentMappingFor(TargetAction);
    }

    public void AssignButtonPressed()
    {
        EmitSignal(nameof(ListenForAction), TargetAction);
    }
    public void ResetAction()
    {
        Controls.Instance.ResetMapping(TargetAction);
    }
}
