namespace Squiggles.Core.BT;

using Godot;
using SquigglesBT;
using SquigglesBT.Nodes;

/// <summary>
/// A behaviour tree node which applies a given force onto the actor.
/// Params:
/// - `dir` : Vec3 -- the direction of the force
/// - `speed` : float -- the magnitude of the force
/// - `is_impulse` : bool -- whether the force is applied as a "CentralForce" or a "CentralImpulse"
/// </summary>
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
