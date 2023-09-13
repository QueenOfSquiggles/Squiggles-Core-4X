using System;
using System.Collections.Generic;
using Godot;

public class SetRandomDirection : Leaf
{
    private Random _Random = new();
    private Vector3 _LastDir = Vector3.Zero;

    protected override void RegisterParams()
    {
        Params["target"] = "key";
    }

    public override int Tick(Node actor, Blackboard bb)
    {
        var target = GetParam("target", "key", bb).AsString();
        var dir = new Vector3
        {
            X = (_Random.NextSingle() - 0.5f) * 2.0f,
            Z = (_Random.NextSingle() - 0.5f) * 2.0f
        }.Normalized();
        bb.SetLocal(target, dir);
        _LastDir = dir;
        return SUCCESS;
    }

    public override void LoadDebuggingValues(Blackboard bb)
    {
        bb.SetLocal($"debug.{Label}:last_dir", _LastDir);
    }
}