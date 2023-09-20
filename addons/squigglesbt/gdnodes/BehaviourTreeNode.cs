namespace SquigglesBT.Nodes;

using Godot;
using Godot.Collections;

public partial class BehaviourTreeNode : Node {
  public enum BTProcessMode { PROCESS, PHYSICS, CUSTOM }
  public enum BTStatus { SUCCESS, FAILURE, RUNNING, ERROR }

  [Export] private Node _actor;
  [Export] private BehaviourTree _behaviour;
  [Export] private BTDebugPanel _debuggingPanel;
  [Export] private BTProcessMode _btProcessMode = BTProcessMode.PROCESS;
  [Export] private Dictionary<string, Variant> _initialBlackboard = new();
  /// <summary>
  /// A debugging value for checking the current running status of the tree.
  /// </summary>
  [Export] private BTStatus _currentBTStatus = BTStatus.SUCCESS;
  [Export] private float _customTickRateSeconds = 0.4f;

  private double _customTickTimer;

  private readonly Blackboard _blackboard = new();

  public override void _Ready() {
    foreach (var entry in _initialBlackboard) {
      _blackboard.SetLocal(entry.Key, entry.Value);
    }

    SetProcess(_btProcessMode is BTProcessMode.PROCESS or BTProcessMode.CUSTOM);
    SetPhysicsProcess(_btProcessMode == BTProcessMode.PHYSICS);

    _behaviour?.RebuildTree();
  }

  public override void _Process(double delta) {
    if (_btProcessMode == BTProcessMode.CUSTOM) {
      _customTickTimer += delta;
      if (_customTickTimer < _customTickRateSeconds) {
        return;
      }

      _customTickTimer = 0;
    }
    DoTick((float)delta);
  }
  public override void _PhysicsProcess(double delta) => DoTick((float)delta);

  private void DoTick(float delta) {
    if (_actor is null || _behaviour is null) {
      return;
    }

    _blackboard.SetLocal("delta", delta);

#if DEBUG
    if (_debuggingPanel is null) {
      _ = _behaviour.TreeRoot.Tick(_actor, _blackboard);
    }
    else {
      var result = _behaviour.TreeRoot.Tick(_actor, _blackboard);
      _currentBTStatus = result switch {
        0 => BTStatus.SUCCESS,
        1 => BTStatus.FAILURE,
        2 => BTStatus.RUNNING,
        _ => BTStatus.ERROR
      };
      _debuggingPanel?.UpdateDisplay(_blackboard);
    }
#else
        _ = _Behaviour.TreeRoot.Tick(_Actor, _Blackboard);
#endif
  }


}
