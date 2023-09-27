namespace Squiggles.FSMTest;

using Godot;
using Squiggles.Core.FSM;

/// <summary>
/// An example of how Squiggles.Core.FSM types can be used to orchestrate logic.
/// </summary>
public partial class CharacterTest : CharacterBody3D {

  [Export] private FiniteStateMachine _stateMachine;
  [Export] private HFSM_State _tpsStates;

  [ExportGroup("Individual States", "_state")]
  [Export] private StateFPS _stateFPS;
  [Export] private StateLongview _stateLongView;
  [Export] private StateOTS _stateOTS;

  public override void _Ready() {

    // top level transitions
    _stateFPS.OnStateFinished += () => _stateMachine.ChangeState(_tpsStates);
    _tpsStates.OnStateFinished += () => _stateMachine.ChangeState(_stateFPS);

    // third person (shooter) transitions
    _stateLongView.OnHoldMouse += () => _tpsStates.ChangeState(_stateOTS);
    _stateOTS.OnReleaseMouse += () => _tpsStates.ChangeState(_stateLongView);

    // capture mouse
    Input.MouseMode = Input.MouseModeEnum.Captured;
  }
}
