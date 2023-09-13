using System.Collections.Generic;
using Godot;

public class Inverter : Decorator
{
    public override int Tick(Node actor, Blackboard blackboard)
    {
        if (Children.Count <= 0) return FAILURE;
        return Children[0].Tick(actor, blackboard) switch
        {
            FAILURE => SUCCESS,
            SUCCESS => FAILURE,
            RUNNING => RUNNING,
            _ => ERROR
        };
    }

    protected override void RegisterParams()
    {
    }

}