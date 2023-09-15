namespace Squiggles.Core.BT;
using Godot;
using Squiggles.Core.Error;
using SquigglesBT;
using SquigglesBT.Nodes;

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
