namespace Squiggles.Core.Sequencing;

using Godot;
using Squiggles.Core.Error;

/// <summary>
/// An implementation of SequenceActionBase that spawns in one or more PackedScenes.
/// </summary>
[GlobalClass]
public partial class SequenceActionSpawnable : SequenceActionBase {
  /// <summary>
  /// Export variable to allow setting the scenes from editor.
  /// </summary>
  [Export] private PackedScene[] _scenes;

  /// <summary>
  /// Spawns all available packed scenes.
  /// </summary>
  /// <param name="owner"></param>
  public override void PerformAction(Node owner) {
    foreach (var p in _scenes) {
      Print.Debug($"Spawning scene: {p.ResourcePath}", this);
      var scene = p.Instantiate();
      owner.AddChild(scene);
    }
  }


}
