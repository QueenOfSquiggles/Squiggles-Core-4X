using System.Collections.Generic;
using Godot;

public class TimeLimiter : Decorator
{


    private float counter = int.MaxValue;


    protected override void RegisterParams()
    {
        Params["seconds"] = 1.0f;
    }

    public override int Tick(Node actor, Blackboard bb)
    {
        if (Children.Count <= 0) return FAILURE;
        var p_counter = GetParam("seconds", 0.0f, bb).AsSingle();
        if (counter > p_counter) counter = p_counter;

        if (counter <= 0) return FAILURE;
        counter -= bb.GetLocal("delta").AsSingle();
        var result = Children[0].Tick(actor, bb);
        if (result != RUNNING) counter = -1;
        return result;
    }

    public override void LoadDebuggingValues(Blackboard bb)
    {
        bb.SetLocal($"debug.{Label}:counter", counter);
    }
}