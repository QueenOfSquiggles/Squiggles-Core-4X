namespace Squiggles.Core.Sequencing;

using System;
using System.Linq;
using Godot;
using Squiggles.Core.Error;
using Squiggles.Core.Extension;

/// <summary>
/// A sequence trigger common to 3D games where an invisible collider is used to trigger any number of actions.
/// </summary>
[GlobalClass]
public partial class SequenceTriggerArea3D : Area3D, ISequenceTrigger {

  /// <summary>
  /// A signal that is triggered when this trigger's validation state changes
  /// </summary>
  /// <param name="isValid"></param>
  [Signal] public delegate void OnTriggerValidationChangeEventHandler(bool isValid);

  /// <summary>
  /// Whether or not to QueueFree when the <see cref="_sequenceActions"/> array is empty. In conjunction with OneShot SequenceActions, can clean up itself after completing the sequence.
  /// </summary>
  [Export] private bool _freeOnQueueEmpty = true;
  /// <summary>
  /// Treated as the "owner" in the Sequencing objects. This is the node to target for a action that uses a node reference. Ideally not the SequenceTrigger itself in case the trigger has <see cref="_freeOnQueueEmpty"/> set to true.
  /// </summary>
  [Export] private Node _actionsTarget;
  /// <summary>
  /// An array of all <see cref="SequenceActionBase"/>s that should be executed.
  /// </summary>
  [Export] private SequenceActionBase[] _sequenceActions = Array.Empty<SequenceActionBase>();

  /// <summary>
  /// A custom filtering for this trigger. Allows for a series of node groups to be defined. Only nodes that are in at least one of the listed groups are considered during collision detection.
  /// </summary>
  [ExportGroup("Filtering", "_filter")]
  [Export] private string[] _filterGroupsAllowed = Array.Empty<string>();


  private bool _isTriggered;

  /// <inheritdoc/>
  public bool GetValidationState() => _isTriggered;


  public override void _Ready() {
    BodyEntered += ProcessNodeEnter;
    AreaEntered += ProcessNodeEnter;

    BodyExited += ProcessNodeExit;
    AreaExited += ProcessNodeExit;
    SetIsTriggered(false);
  }

  private void SetIsTriggered(bool value) {
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

  private void ProcessNodeEnter(Node node) {
    if (IsNodeValid(node)) {
      SetIsTriggered(true);
    }
  }
  private void ProcessNodeExit(Node node) {
    if (IsNodeValid(node)) {
      SetIsTriggered(false);
    }
  }

  private bool IsNodeValid(Node node) {
    if (node is not Node3D n3d) {
      Print.Debug($"Node not valid, is not Node3D: {node}");
      return false;
    }
    if (_filterGroupsAllowed.Length > 0) {
      var flag = false;
      foreach (var g in _filterGroupsAllowed) {
        if (n3d.IsInGroup(g)) {
          flag = true;
          break;
        }
      }
      if (!flag) {
        Print.Debug($"Node not valid, is not part of any filter groups: Node[{node.GetGroups().ToArray().ToDebugString()}], Filter[{_filterGroupsAllowed.ToDebugString()}]");
        return false;
      }
    }
    Print.Debug($"Found a valid object! {node} -> {node?.Name}");
    return true;
  }
}
