namespace SquigglesBT.Nodes;
using Godot;

public class Succeeder : Decorator {
  public override int Tick(Node actor, Blackboard blackboard) {
    if (Children.Count <= 0) {
      return FAILURE;
    }

    Children[0].Tick(actor, blackboard);
    return SUCCESS;
  }
  protected override void RegisterParams() {
  }

}
