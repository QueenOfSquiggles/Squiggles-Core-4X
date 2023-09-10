using Godot;
using queen.data;
using System;

public partial class ForceAccessLoad : Node
{

    public override void _Ready()
    {
        var _ = Access.Instance.FontOption; // forcing a load of the instance
    }
}
