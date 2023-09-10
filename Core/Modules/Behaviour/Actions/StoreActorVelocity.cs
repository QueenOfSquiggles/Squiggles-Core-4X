using System;
using System.Collections.Generic;
using Godot;
using queen.extension;

public class StoreActorVelocity : Leaf
{
    protected override void RegisterParams()
    {
        Params["store_as"] = "key";
    }

    public override int Tick(Node actor, Blackboard bb)
    {
        if (actor is not RigidBody3D rb) return FAILURE;
        var store_as = GetParam("store_as", "key", bb).AsString();
        bb.SetLocal(store_as, rb.LinearVelocity);
        return SUCCESS;
    }

    public override void LoadDebuggingValues(Blackboard bb)
    {
        bb.SetLocal($"debug.{Label}:last_dir", bb.GetLocal(GetParam("target", "key", bb).AsString()));
    }
}