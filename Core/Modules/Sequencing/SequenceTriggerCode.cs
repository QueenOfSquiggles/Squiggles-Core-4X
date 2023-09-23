namespace Squiggles.Core.Sequencing;

using System;
using System.Linq;
using Godot;
using Squiggles.Core.Error;
using Squiggles.Core.Extension;

[GlobalClass]
public partial class SequenceTriggerCode : Node, ISequenceTrigger
{

  [Signal] public delegate void OnTriggerValidationChangeEventHandler(bool isValid);

  [Export] private bool _freeOnQueueEmpty = true;
  [Export] private Node _actionsTarget;
  [Export] private SequenceActionBase[] _sequenceActions = Array.Empty<SequenceActionBase>();

  private bool _isTriggered;
  public bool GetValidationState() => _isTriggered;


  public override void _Ready()
  {
    SetIsTriggered(false);
  }


  public void SetIsTriggered(bool value)
  {
    if (value == _isTriggered)
    {
      return; // no changes to make
    }

    _isTriggered = value;
    EmitSignal(nameof(OnTriggerValidationChange), _isTriggered);

    if (!_isTriggered)
    {
      return; // only execute remainder if we are true in validation
    }
    PerformTriggerActions();
  }

  public void PerformTriggerActions()
  {
    var buffer = _sequenceActions.ToList();
    Print.Debug("Performing actions", this);
    foreach (var action in buffer)
    {
      action.PerformAction(_actionsTarget);
      if (action.OneShot)
      {
        // remove action from sequence
        _sequenceActions = _sequenceActions.Where((a) => a != action).ToArray();
      }
    }
    if (_freeOnQueueEmpty && _sequenceActions.Length <= 0)
    {
      Print.Debug("Deleting self as no actions are remaining in queue", this);
      QueueFree();
    }
  }

}
