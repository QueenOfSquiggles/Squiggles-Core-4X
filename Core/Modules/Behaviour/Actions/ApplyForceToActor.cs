using System;
using System.Collections.Generic;
using Godot;
using queen.extension;

public class ApplyForceToActor : Leaf
{
    private Vector3 _LastForce = Vector3.Zero;

    protected override void RegisterParams()
    {
        Params["dir"] = Vector3.One;
        Params["speed"] = 1.0f;
        Params["is_impulse"] = false;
    }

    public override int Tick(Node actor, Blackboard bb)
    {
        if (actor is not RigidBody3D rb) return FAILURE;
        var dir = GetParam("dir", Vector3.One, bb).AsVector3();
        var force = dir * GetParam("speed", 1.0f, bb).AsSingle();

        if (GetParam("is_impulse", false, bb).AsBool()) rb.ApplyCentralImpulse(force);
        else rb.ApplyCentralForce(force);

        _LastForce = force;
        return SUCCESS;
    }

    public override void LoadDebuggingValues(Blackboard bb)
    {
        bb.SetLocal($"debug.{Label}:last_force", _LastForce);
    }

}