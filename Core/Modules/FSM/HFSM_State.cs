namespace Squiggles.Core.FSM;

using Godot;

/// <summary>
/// Serves both the role of <see cref="FiniteStateMachine"/> and <see cref="State"/>. This can be used to create a naive Hierarchical State Machine, which allows nesting smaller state machines within a larger state machine in order to produce more complex behaviour without requiring a mess of transition definitions.
/// </summary>
/// <remarks>
/// I really dislike this name. Anything with an underscore makes me cringe (at least for C# type names). I'm tempted to just write it all out so it's more obvious and removes the underscore.
/// </remarks>
[GlobalClass]
public partial class HFSM_State : State {

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
      _current.IsActive = false;
      _current.ExitState();
      _current.OnStateFinished -= HandleChildStateFinished;
    }
    _current = n_state;
    if (_current is not null) {
      _current.EnterState();
      _current.IsActive = IsActive; // only set active if this node is also active. Prevents inactive nodes performing processing
      _current.OnStateFinished += HandleChildStateFinished;
    }
  }

  public override void EnterState() {
    if (_current is null) {
      ChangeState(_initialState);
    }
    _current?.EnterState();
    _current.IsActive = true;
  }
  public override void ExitState() {
    if (_current is null) {
      return;
    }
    _current.IsActive = false;
    _current.ExitState();
  }

  private void HandleChildStateFinished() => EmitSignal(nameof(OnStateFinished));
}
