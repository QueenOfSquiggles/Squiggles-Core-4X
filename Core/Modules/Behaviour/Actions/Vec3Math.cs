using System;
using System.Collections.Generic;
using Godot;
using queen.extension;

public class Vec3Math : Leaf
{

    private float[] DEFAULT_ARRAY = new float[3];
    protected override void RegisterParams()
    {
        Params["op"] = "float";
        Params["target"] = "key";
        Params["value"] = DEFAULT_ARRAY;
    }

    public override int Tick(Node actor, Blackboard bb)
    {
        var target = GetParam("target", "key", bb).AsString();
        var op = GetParam("op", "+", bb).AsString();
        var value = bb.GetLocal(target).AsVector3();
        var nums = GetParam("value", DEFAULT_ARRAY, bb).AsFloat32Array();
        switch (op)
        {
            case "+":
                bb.SetLocal(target, new Vector3
                {
                    X = value.X + nums[0],
                    Y = value.Y + nums[1],
                    Z = value.Z + nums[2],
                });
                break;
            case "*":
                bb.SetLocal(target, new Vector3
                {
                    X = value.X * nums[0],
                    Y = value.Y * nums[1],
                    Z = value.Z * nums[2],
                });
                break;
            case "-":
                bb.SetLocal(target, new Vector3
                {
                    X = value.X - nums[0],
                    Y = value.Y - nums[1],
                    Z = value.Z - nums[2],
                });
                break;
            case "/":
                bb.SetLocal(target, new Vector3
                {
                    X = value.X / nums[0],
                    Y = value.Y / nums[1],
                    Z = value.Z / nums[2],
                });
                break;

        }

        return SUCCESS;
    }

    public override void LoadDebuggingValues(Blackboard bb)
    {
        bb.SetLocal($"debug.{Label}:last_dir", bb.GetLocal(GetParam("target", "key", bb).AsString()));
    }
}