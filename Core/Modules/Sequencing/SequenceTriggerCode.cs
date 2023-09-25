namespace Squiggles.Core.Sequencing;

using System;
using System.Linq;
using Godot;
using Squiggles.Core.Error;

/// <summary>
/// An implementation of <see cref="ISequenceTrigger"/> that can only be called from code. Useful in some circumstances. Such as using an animation player to call method, or connecting a signal with some default bindings.
/// </summary>
[GlobalClass]
public partial class SequenceTriggerCode : Node, ISequenceTrigger {

  /// <summary>
  /// A signal that is triggered when this trigger's validation state changes
  /// </summary>
  /// <param name="isValid"></param>
  [Signal] public delegate void OnTriggerValidationChangeEventHandler(bool isValid);
  /// <summary>
  /// Whether or not to QueueFree when the <see cref="_sequenceActions"/> array is empty. In conjunction with OneShot SequenceActions, can clean up itself after completing the sequence.
  /// </summary>
  [Export] private bool _freeOnQueueEmpty = true;
  /// Treated as the "owner" in the Sequencing objects. This is the node to target for a action that uses a node reference. Ideally not the SequenceTrigger itself in case the trigger has <see cref="_freeOnQueueEmpty"/> set to true.
  [Export] private Node _actionsTarget;
  /// <summary>
  /// An array of all <see cref="SequenceActionBase"/>s that should be executed.
  /// </summary>
  [Export] private SequenceActionBase[] _sequenceActions = Array.Empty<SequenceActionBase>();

  private bool _isTriggered;
  /// <inheritdoc/>
  public bool GetValidationState() => _isTriggered;


  public override void _Ready() => SetIsTriggered(false);

  /// <summary>
  /// Sets whether the node is currently triggered. Only calls <see cref="PerformTriggerActions"/> if this causes a change in state. Useful if multiple sources could be calling this trigger, but it should only execute once.
  /// </summary>
  /// <param name="value"></param>
  public void SetIsTriggered(bool value) {
    if (value == _isTriggered) {
      return; // no changes to make
    }

    _isTriggered = value;
    EmitSignal(nameof(OnTriggerValidationChange), _isTriggered);

    if (!_isTriggered) {
      return; // only execute remainder if we are true in validation
    }
    PerformTriggerActions();
  }

  /// <summary>
  /// Called to perform all of the stored actions. Exposed publicly in case you wish to call it programmatically.
  /// </summary>
  public void PerformTriggerActions() {
    var buffer = _sequenceActions.ToList();
    Print.Debug("Performing actions", this);
    foreach (var action in buffer) {
      action.PerformAction(_actionsTarget);
      if (action.OneShot) {
        // remove action from sequence
        _sequenceActions = _sequenceActions.Where((a) => a != action).ToArray();
      }
    }
    if (_freeOnQueueEmpty && _sequenceActions.Length <= 0) {
      Print.Debug("Deleting self as no actions are remaining in queue", this);
      QueueFree();
    }
  }

}
