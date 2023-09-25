namespace Squiggles.Core.BT;
using Godot;
using Squiggles.Core.Error;
using SquigglesBT;
using SquigglesBT.Nodes;

/// <summary>
/// A behaviour tree node which queries whether the actor is currently on the ground based on a loaded raycast node in the blackboard
/// Params:
/// - `raycast_key` : string -- the name of the node path loaded into the behaviour tree by the scene node that manages it. Custom implementations beware!
/// </summary>
public class ActorRaycastColliding : Leaf {

  protected override void RegisterParams() => Params["raycast_key"] = "unassigned_path";

  public override int Tick(Node actor, Blackboard blackboard) {
    if (actor is not RigidBody3D rb) {
      return FAILURE;
    }

    var path = GetParam("raycast_key", "", blackboard).AsString();

    if (path == "") {
      Print.Warn($"Failed to find raycast key.");
      return FAILURE;
    }

    var raycast = actor.GetNode<RayCast3D>(path);
    return raycast is null ? FAILURE : raycast.IsColliding() ? SUCCESS : FAILURE;
  }

}
