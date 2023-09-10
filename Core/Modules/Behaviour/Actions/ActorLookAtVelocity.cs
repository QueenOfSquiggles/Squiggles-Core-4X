using System;
using System.Collections.Generic;
using Godot;
using queen.extension;

public class ActorLookAtVelocity : Leaf
{

    protected override void RegisterParams()
    {
        Params["skip-y"] = false;
        Params["model-front"] = true;
    }

    public override int Tick(Node actor, Blackboard bb)
    {
        if (actor is not RigidBody3D rb) return FAILURE;
        var off = rb.LinearVelocity;

        if (GetParam("skip-y", false, bb).AsBool()) off.Y = 0f;
        if (off.LengthSquared() <= 0.5f) return FAILURE;

        rb.LookAt(rb.GlobalPosition - off.Normalized(), Vector3.Up, GetParam("model-front", true, bb).AsBool());
        return SUCCESS;
    }

}