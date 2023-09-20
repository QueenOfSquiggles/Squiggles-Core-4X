namespace SquigglesBT.Nodes;
using Godot;

public class Select : Compositor {
  public override int Tick(Node actor, Blackboard blackboard) {
    foreach (var c in Children) {
      var result = c.Tick(actor, blackboard);
      if (result != FAILURE) {
        return result;
      }
    }
    return FAILURE;
  }

  protected override void RegisterParams() {
  }

}
