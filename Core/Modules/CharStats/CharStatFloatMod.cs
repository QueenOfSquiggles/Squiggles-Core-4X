using System;
using Godot;

public partial class CharStatFloatMod : CharStatFloat
{

    [Export] public float Duration = 1.0f;

    public override void _Ready()
    {
        base._Ready();
        DeathClock();
    }

    protected async void DeathClock()
    {
        var timer = GetTree().CreateTimer(Duration);
        await ToSignal(timer, "timeout");
        QueueFree();
    }
}
