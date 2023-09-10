using Godot;
using queen.events;
using System;

public partial class CinematicSequence : Node
{

    public void Start()
    {
        Events.Gameplay.TriggerRequestPlayerAbleToMove(false);
    }

    public void End()
    {
        Events.Gameplay.TriggerRequestPlayerAbleToMove(true);
    }
}
