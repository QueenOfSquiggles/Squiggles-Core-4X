namespace Squiggles.Core.BT;
using Godot;
using SquigglesBT;
using SquigglesBT.Nodes;

/// <summary>
/// A behaviour tree which stores the length of a Vector3 property. Ideally loaded from the blackboard since this serves no purpose otherwise.
/// Params:
/// `target` : Vector3 -- the target Vec3 to process. Note that prefixing this property's value with a "$" and following with a string that is the name of a variable stored in the blackboard will attempt to load that property in as the value instead. For example "$actor_velocity"
/// - `store_as` : string -- the target to store the result as
/// - `squared` : bool -- whether or not to use the length_squared instead of just length. It is more performant to use squared but you would need to take that into account.
/// </summary>
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
