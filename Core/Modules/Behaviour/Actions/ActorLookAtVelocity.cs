namespace Squiggles.Core.BT;

using Godot;
using SquigglesBT;
using SquigglesBT.Nodes;

/// <summary>
/// A behaviour tree action that aligns the actor look with the actor's velocity. It does assume the actor is a <c>RigidBody3D</c> and fails if not.
/// Properties include:
/// - `skip-y` : bool -- whether or not to ignore the Y axis of the movement vector (removes XZ rotations when enabled)
/// - `model-front` : bool -- whether or not to use the "model front" flag in Godot's <c>Node3D.<see href="https://docs.godotengine.org/en/stable/classes/class_node3d.html#class-node3d-method-look-at">LookAt</see></c>
/// </summary>
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
