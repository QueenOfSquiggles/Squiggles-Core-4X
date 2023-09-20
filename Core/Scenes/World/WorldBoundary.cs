namespace Squiggles.Core.Scenes.World;

using Godot;

public partial class WorldBoundary : Area3D {
  [Export] private string _playerGroupName = "player";
  [Export] private Node3D _respawnPoint;

  private void OnBodyEnter(Node3D node) {
    if (_respawnPoint is null) {
      return;
    }

    node.GlobalPosition = _respawnPoint.GlobalPosition;
  }
}
