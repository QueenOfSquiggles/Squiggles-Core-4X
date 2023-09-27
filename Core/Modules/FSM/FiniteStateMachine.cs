namespace Squiggles.Core.FSM;

using Godot;

/// <summary>
/// The root node for state machine structures. Merely serves as a controller for switching between active states. Transitions must be handled by an external script for example by a player controller that  contains this FSM.
/// </summary>
[GlobalClass]
public partial class FiniteStateMachine : Node {

  [Export] private State _initialState;

  private State _current;

  public override void _Ready() {
    // reduce overhead
    SetProcess(false);
    SetPhysicsProcess(false);

    // load inital state
    ChangeState(_initialState);
  }

  public virtual void ChangeState(State n_state) {
    if (_current is not null) {
      _current.ExitState();
      _current.IsActive = false;
    }

    _current = n_state;

    if (_current is not null) {
      _current?.EnterState();
      _current.IsActive = true;
    }
  }
}
