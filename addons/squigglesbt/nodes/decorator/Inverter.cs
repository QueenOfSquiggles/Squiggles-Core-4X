namespace SquigglesBT.Nodes;
using Godot;

public class Inverter : Decorator {
  public override int Tick(Node actor, Blackboard blackboard)
    => Children.Count <= 0
      ? FAILURE
      : Children[0].Tick(actor, blackboard) switch {
        FAILURE => SUCCESS,
        SUCCESS => FAILURE,
        RUNNING => RUNNING,
        _ => ERROR
      };

  protected override void RegisterParams() {
  }

}
