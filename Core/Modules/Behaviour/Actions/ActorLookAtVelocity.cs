namespace Squiggles.Core.BT;

using Godot;
using SquigglesBT;
using SquigglesBT.Nodes;

public class ActorLookAtVelocity : Leaf {
  public ActorLookAtVelocity() { }

  protected override void RegisterParams() {
    Params["skip-y"] = false;
    Params["model-front"] = true;
  }

  public override int Tick(Node actor, Blackboard blackboard) {
    if (actor is not RigidBody3D rb) {
      return FAILURE;
    }

    var off = rb.LinearVelocity;

    if (GetParam("skip-y", false, blackboard).AsBool()) {
      off.Y = 0f;
    }

    if (off.LengthSquared() <= 0.5f) {
      return FAILURE;
    }

    rb.LookAt(rb.GlobalPosition - off.Normalized(), Vector3.Up, GetParam("model-front", true, blackboard).AsBool());
    return SUCCESS;
  }

}
