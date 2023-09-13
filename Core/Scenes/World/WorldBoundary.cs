using System;
using Godot;

public partial class WorldBoundary : Area3D
{
    [Export] private string PlayerGroupName = "player";
    [Export] private Node3D _RespawnPoint;

    private void OnBodyEnter(Node3D node)
    {
        if (_RespawnPoint is null) return;
        node.GlobalPosition = _RespawnPoint.GlobalPosition;
    }
}
