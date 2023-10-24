namespace Squiggles.Core.Scenes.Interactables;

using Godot;
using Squiggles.Core.Interaction;

/// <summary>
/// An Area3D that implements <see cref="IInteratctable"/> and <see cref="ISelectable"/>. Add this component to an object to allow it to be treated as an interactable object.
/// </summary>
[GlobalClass]
public partial class InteractiveTrigger : Area3D, IInteractable, ISelectable {

  /// <summary>
  /// Whether or not this interactive object is currently active.
  /// </summary>
  [Export] public bool IsActive = true;
  /// <summary>
  /// A custom name to display when selected
  /// </summary>
  [Export] public string CustomName = "";
  /// <summary>
  /// A signal emitted when this <see cref="IInteractable"/> is interacted with.
  /// </summary>
  [Signal] public delegate void OnInteractedEventHandler();

  /// <summary>
  /// Called when this <see cref="ISelectable"/> is selected
  /// </summary>
  [Signal] public delegate void OnSelectedEventHandler();

  /// <summary>
  /// Called when this <see cref="ISelectable"/> is deselected
  /// </summary>
  [Signal] public delegate void OnDeselectedEventHandler();

  /// <summary>
  /// Returns the current custom name, virtual so you can override with your own logic.
  /// </summary>
  /// <returns>the <see cref="CustomName"/> if defined, else the Name on the node</returns>
  public virtual string GetActiveName() => CustomName.Length > 0 ? CustomName : Name;

  public virtual bool Interact() {
    EmitSignal(nameof(OnInteracted));
    return true;
  }

  public virtual bool GetIsActive() => IsActive;

  public void OnSelect() => EmitSignal(nameof(OnSelected));

  public void OnDeselect() => EmitSignal(nameof(OnDeselected));
}
