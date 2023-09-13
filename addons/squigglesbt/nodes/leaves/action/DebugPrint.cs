using System.Collections.Generic;
using Godot;

public class DebugPrint : Leaf
{

    protected override void RegisterParams()
    {
        Params["message"] = "Message UwU";
    }

    public override int Tick(Node actor, Blackboard blackboard)
    {
        var msg = GetParam("message", "", blackboard).AsString();
        if (msg == "") return FAILURE;
        GD.Print(msg);
        return SUCCESS;
    }
}