using Godot;
using queen.data;
using queen.extension;
using System;

public partial class ActionMappingSlot : HBoxContainer
{

    [Signal] public delegate void ListenForActionEventHandler(string action_name);

    [Export] private string TargetAction = "";

    private Label label;
    private Button action_button;
    public override void _Ready()
    {
        this.GetNode("Label", out label);
        this.GetNode("BtnListen", out action_button);

        label.Text = TargetAction.Replace("_", " ");
        action_button.Text = Controls.Instance.GetCurrentMappingFor(TargetAction);
        Controls.Instance.OnControlMappingChanged += HandleMappingChanged;
    }

    public override void _ExitTree()
    {
        Controls.Instance.OnControlMappingChanged -= HandleMappingChanged;
    }

    private void HandleMappingChanged(string action)
    {
        if(action != TargetAction) return;
        action_button.Text = Controls.Instance.GetCurrentMappingFor(TargetAction);        
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
