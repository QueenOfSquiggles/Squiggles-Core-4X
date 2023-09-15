namespace Squiggles.Core.Scenes.Interactables;

using Godot;
using Squiggles.Core.Interaction;

public partial class InteractiveTrigger : Area3D, IInteractable, ISelectable {

  [Export] public bool IsActive = true;
  [Export] public string CustomName = "";
  [Signal] public delegate void OnInteractedEventHandler();
  [Signal] public delegate void OnSelectedEventHandler();
  [Signal] public delegate void OnDeselectedEventHandler();

  public virtual string GetActiveName() => CustomName.Length > 0 ? CustomName : Name;

  public virtual bool Interact() {
    EmitSignal(nameof(OnInteracted));
    return true;
  }

  public virtual bool GetIsActive() => IsActive;

  public void OnSelect() => EmitSignal(nameof(OnSelected));

  public void OnDeselect() => EmitSignal(nameof(OnDeselected));
}
