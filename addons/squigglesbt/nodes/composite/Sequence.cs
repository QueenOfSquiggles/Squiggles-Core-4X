using System.Collections.Generic;
using Godot;

public class Sequence : Compositor
{
    public override int Tick(Node actor, Blackboard blackboard)
    {
        foreach (var c in Children)
        {
            var result = c.Tick(actor, blackboard);
            if (result != SUCCESS) return result;
        }
        return SUCCESS;
    }

    protected override void RegisterParams()
    {
    }

}