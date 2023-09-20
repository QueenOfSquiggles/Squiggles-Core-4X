namespace SquigglesBT.Nodes;
using Godot;

public class Root : BTNode {
  public Root() : base("Root", 1) { }

  public override int Tick(Node actor, Blackboard blackboard) {
    if (Children.Count <= 0) { GD.PushWarning("Running empty behaviour tree"); return FAILURE; }
    return Children[0].Tick(actor, blackboard);
  }

  protected override void RegisterParams() {
  }
}
