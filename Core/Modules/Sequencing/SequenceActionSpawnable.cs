namespace Squiggles.Core.Sequencing;

using Godot;
using Squiggles.Core.Error;

[GlobalClass]
public partial class SequenceActionSpawnable : SequenceActionBase {
  [Export] private PackedScene[] _scenes;

  public override void PerformAction(Node owner) {
    foreach (var p in _scenes) {
      Print.Debug($"Spawning scene: {p.ResourcePath}", this);
      var scene = p.Instantiate();
      owner.AddChild(scene);
    }
  }


}
