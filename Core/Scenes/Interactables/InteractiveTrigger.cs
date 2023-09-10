using System;
using Godot;
using interaction;
using Modules.Interaction;
using queen.error;

public partial class InteractiveTrigger : Area3D, IInteractable, ISelectable
{

    [Export] public bool is_active = true;
    [Export] public string custom_name = "";
    [Signal] public delegate void OnInteractedEventHandler();
    [Signal] public delegate void OnSelectedEventHandler();
    [Signal] public delegate void OnDeselectedEventHandler();

    public virtual string GetActiveName()
    {
        return custom_name.Length > 0 ? custom_name : Name;
    }

    public virtual bool Interact()
    {
        EmitSignal(nameof(OnInteracted));
        return true;
    }

    public virtual bool IsActive()
    {
        return is_active;
    }

    public void OnSelect() => EmitSignal(nameof(OnSelected));

    public void OnDeselect() => EmitSignal(nameof(OnDeselected));
}
