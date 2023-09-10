using System;
using System.Collections.Generic;
using Godot;
using queen.extension;

public class StoreVec3Length : Leaf
{
    protected override void RegisterParams()
    {
        Params["target"] = Vector3.Zero;
        Params["store_as"] = "key";
        Params["squared"] = false;
    }

    public override int Tick(Node actor, Blackboard bb)
    {
        var target = GetParam("target", "key", bb).AsVector3();
        var store_as = GetParam("store_as", "key", bb).AsString();
        var squared = GetParam("squared", false, bb).AsBool();

        if (squared) bb.SetLocal(store_as, target.LengthSquared());
        else bb.SetLocal(store_as, target.Length());
        return SUCCESS;
    }

    public override void LoadDebuggingValues(Blackboard bb)
    {
        bb.SetLocal($"debug.{Label}:last_dir", bb.GetLocal(GetParam("target", "key", bb).AsString()));
    }
}