using System;
using System.Collections.Generic;
using Godot;
using queen.error;
using queen.extension;

public class ActorPlayAnim : Leaf
{

    protected override void RegisterParams()
    {
        Params["track"] = "track name";
        Params["reverse"] = false;
    }

    public override int Tick(Node actor, Blackboard bb)
    {
        var path = bb.GetLocal("anim").AsNodePath();
        if (actor.GetNode(path) is not AnimationPlayer anim)
        {
            Print.Error($"Failed to find node from path {path} for behaviour tree");
            return FAILURE;
        }
        var track = GetParam("track", "track_name", bb).AsString();

        if (GetParam("reverse", false, bb).AsBool()) anim.PlayBackwards(track);
        else anim.Play(track);

        return SUCCESS;
    }

}