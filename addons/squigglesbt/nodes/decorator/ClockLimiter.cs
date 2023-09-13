using System.Collections.Generic;
using Godot;

public class ClockLimiter : Decorator
{
    private float counter = float.MaxValue;


    protected override void RegisterParams()
    {
        Params["seconds"] = 1.0f;
        Params["startat"] = 0.0f;
    }

    public override int Tick(Node actor, Blackboard bb)
    {
        if (Children.Count <= 0 || !Params.ContainsKey("seconds")) return FAILURE;
        var p_counter = GetParam("seconds", 0.0f, bb).AsSingle();
        if (counter > p_counter) counter = p_counter;

        counter -= bb.GetLocal("delta").AsSingle();
        if (counter <= 0f)
        {
            var result = Children[0].Tick(actor, bb);
            if (result != RUNNING) counter = p_counter;
            return result;
        }
        return SUCCESS;
    }

    public override void LoadDebuggingValues(Blackboard bb)
    {
        bb.SetLocal($"debug.{Label}:counter", counter);
    }
}