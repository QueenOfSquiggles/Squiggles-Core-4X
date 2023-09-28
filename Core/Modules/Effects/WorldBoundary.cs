namespace Squiggles.Core.Scenes.World;

using Godot;
using Squiggles.Core.Attributes;

/// <summary>
/// A utility area that allows creating a boundary, where any object that falls in "respawns" by teleporting to the specified location. Helpful for quickly making "infinite pits" or whatever you expect the player to fall into
/// </summary>
[GlobalClass]
public partial class WorldBoundary : Area3D {
  /// <summary>
  /// The node group that marks the player. Currently unused actually
  /// </summary>
  [MarkForRefactor("Unused value", "Why is this here?")]
  [Export] private string _playerGroupName = "player";
  /// <summary>
  /// The target node that provides the position at which to respawn objects as well as the player.
  /// </summary>
  [Export] private Node3D _respawnPoint;

  private void OnBodyEnter(Node3D node) {
    if (_respawnPoint is null) {
      return;
    }

    node.GlobalPosition = _respawnPoint.GlobalPosition;
  }
}
