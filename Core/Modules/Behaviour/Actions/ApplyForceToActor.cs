namespace Squiggles.Core.BT;

using Godot;
using SquigglesBT;
using SquigglesBT.Nodes;

public class ApplyForceToActor : Leaf {
  private Vector3 _lastForce = Vector3.Zero;

  protected override void RegisterParams() {
    Params["dir"] = Vector3.One;
    Params["speed"] = 1.0f;
    Params["is_impulse"] = false;
  }

  public override int Tick(Node actor, Blackboard blackboard) {
    if (actor is not RigidBody3D rb) {
      return FAILURE;
    }

    var dir = GetParam("dir", Vector3.One, blackboard).AsVector3();
    var force = dir * GetParam("speed", 1.0f, blackboard).AsSingle();

    if (GetParam("is_impulse", false, blackboard).AsBool()) {
      rb.ApplyCentralImpulse(force);
    }
    else {
      rb.ApplyCentralForce(force);
    }

    _lastForce = force;
    return SUCCESS;
  }

  public override void LoadDebuggingValues(Blackboard bb) => bb.SetLocal($"debug.{Label}:last_force", _lastForce);

}
