using System;
using System.Collections.Generic;
using Godot;
using queen.error;
using queen.extension;

public class ActorRaycastColliding : Leaf
{

    protected override void RegisterParams()
    {
        Params["raycast_key"] = "unassigned_path";
    }

    public override int Tick(Node actor, Blackboard bb)
    {
        if (actor is not RigidBody3D rb) return FAILURE;
        var path = GetParam("raycast_key", "", bb).AsString();
        if (path == "") { Print.Warn($"Failed to find raycast key."); return FAILURE; }
        var raycast = actor.GetNode<RayCast3D>(path);
        if (raycast is null) return FAILURE;
        return raycast.IsColliding() ? SUCCESS : FAILURE;
    }

}