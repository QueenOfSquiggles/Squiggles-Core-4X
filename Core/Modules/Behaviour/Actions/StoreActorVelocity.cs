namespace Squiggles.Core.BT;
using Godot;
using SquigglesBT;
using SquigglesBT.Nodes;

public class StoreActorVelocity : Leaf {
  protected override void RegisterParams() => Params["store_as"] = "key";

  public override int Tick(Node actor, Blackboard blackboard) {
    if (actor is not RigidBody3D rb) {
      return FAILURE;
    }

    var store_as = GetParam("store_as", "key", blackboard).AsString();
    blackboard.SetLocal(store_as, rb.LinearVelocity);
    return SUCCESS;
  }

  public override void LoadDebuggingValues(Blackboard bb)
    => bb.SetLocal($"debug.{Label}:last_dir", bb.GetLocal(GetParam("target", "key", bb).AsString()));
}
