namespace SquigglesBT.Nodes;
using Godot;

public class Failer : Decorator {
  public override int Tick(Node actor, Blackboard blackboard) {
    if (Children.Count <= 0) {
      return FAILURE;
    }

    Children[0].Tick(actor, blackboard);
    return FAILURE;
  }

  protected override void RegisterParams() {
  }

}
