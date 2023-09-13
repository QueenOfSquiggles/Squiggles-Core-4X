using System.Collections.Generic;
using Godot;

public class BlackboardHas : Leaf
{
    protected override void RegisterParams()
    {
        Params["key"] = "bb_key";
    }

    public override int Tick(Node actor, Blackboard blackboard)
    {
        var key = GetParam("key", "key", blackboard).AsString();
        return blackboard.HasLocal(key) ? SUCCESS : FAILURE;
    }
}