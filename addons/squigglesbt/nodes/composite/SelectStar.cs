using System.Collections.Generic;
using Godot;

public class SelectStar : Compositor
{
    private int _Current = -1;

    public override int Tick(Node actor, Blackboard blackboard)
    {
        if (_Current >= 0)
        {
            var result = Children[0].Tick(actor, blackboard);
            if (result != RUNNING) _Current = -1;
            return result;
        }
        foreach (var c in Children)
        {
            var result = c.Tick(actor, blackboard);
            if (result == RUNNING) _Current = Children.IndexOf(c);
            if (result != SUCCESS) return result;
        }
        return SUCCESS;
    }

    protected override void RegisterParams()
    {
    }

}