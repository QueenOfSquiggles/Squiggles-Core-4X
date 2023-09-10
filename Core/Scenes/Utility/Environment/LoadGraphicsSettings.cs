using Godot;
using queen.data;
using System;

public partial class LoadGraphicsSettings : Node
{

    public override void _Ready()
    {
        var _ = Graphics.Instance.SDFGI; // forces instance initialization
    }
}
