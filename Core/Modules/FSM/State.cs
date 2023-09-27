namespace Squiggles.Core.FSM;

using System;
using Godot;

/// <summary>
/// An individual state for an FSM (<see cref="FiniteStateMachine"/>). Extend and override <see cref="EnterState"/>, <see cref="ExitState"/>, as well as <see cref="_Process"/> or <see cref="_PhysicsProcess"/> to impelement functionality. An important note is that this node (by default) does not use _Process or _PhysicsProcess through calling SetProcess(false) and SetPhysicsProcess(false) during the _Ready method. While this reduces idle processing draw, you will need to specify if you want to use one of these methods. Ideally turning it on/off in the enter/exit functions respectively.
/// </summary>
/// <remarks>
/// If you're familair with HeartBeast's recent FSM tutorial, it should be pretty obvious where I got my ideas from.
/// </remarks>
[GlobalClass]
public partial class State : Node {

  /// <summary>
  /// A single standard signal to show that this state must be transitioned away from. For more specificity, individual states can implement their own signals which need to be handled for transitions to work.
  /// </summary>
  [Signal] public delegate void OnStateFinishedEventHandler();

  /// <summary>
  /// A property that determines if this state is the active state. Use this to block off logic that shouldn't operate while this state is inactive.
  /// </summary>
  public bool IsActive { get; internal set; }

  public override void _Ready() {
    SetProcess(false);
    SetPhysicsProcess(false);
  }
  /// <summary>
  /// The method which is called when this state becomes active. Extend this to implement custom functionality.
  /// </summary>
  /// <exception cref="NotImplementedException"></exception>
  public virtual void EnterState() => throw new NotImplementedException("State must be extended and overridden to create functionality. Base state class is unusafe for FSM actions");

  /// <summary>
  /// The method which is called when this state becomes inactive. Extend this to implement custom functionality.
  /// </summary>
  /// <exception cref="NotImplementedException"></exception>
  public virtual void ExitState() => throw new NotImplementedException("State must be extended and overridden to create functionality. Base state class is unusafe for FSM actions");
}
