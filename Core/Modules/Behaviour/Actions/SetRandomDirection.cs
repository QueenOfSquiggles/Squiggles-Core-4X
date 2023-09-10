using System;
using System.Collections.Generic;
using Godot;

public class SetRandomDirection : Leaf
{
    private Random rand = new();
    private Vector3 LastDir = Vector3.Zero;

    protected override void RegisterParams()
    {
        Params["target"] = "key";
    }

    public override int Tick(Node actor, Blackboard bb)
    {
        var target = GetParam("target", "key", bb).AsString();
        var dir = new Vector3
        {
            X = (rand.NextSingle() - 0.5f) * 2.0f,
            Z = (rand.NextSingle() - 0.5f) * 2.0f
        }.Normalized();
        bb.SetLocal(target, dir);
        LastDir = dir;
        return SUCCESS;
    }

    public override void LoadDebuggingValues(Blackboard bb)
    {
        bb.SetLocal($"debug.{Label}:last_dir", LastDir);
    }
}