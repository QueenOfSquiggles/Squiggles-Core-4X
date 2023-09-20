namespace Squiggles.Core.BT;
using Godot;
using SquigglesBT;
using SquigglesBT.Nodes;

public class StoreVec3Length : Leaf {
  protected override void RegisterParams() {
    Params["target"] = Vector3.Zero;
    Params["store_as"] = "key";
    Params["squared"] = false;
  }

  public override int Tick(Node actor, Blackboard blackboard) {
    var target = GetParam("target", "key", blackboard).AsVector3();
    var store_as = GetParam("store_as", "key", blackboard).AsString();
    var squared = GetParam("squared", false, blackboard).AsBool();

    if (squared) {
      blackboard.SetLocal(store_as, target.LengthSquared());
    }
    else {
      blackboard.SetLocal(store_as, target.Length());
    }

    return SUCCESS;
  }

  public override void LoadDebuggingValues(Blackboard bb)
    => bb.SetLocal($"debug.{Label}:last_dir", bb.GetLocal(GetParam("target", "key", bb).AsString()));
}
