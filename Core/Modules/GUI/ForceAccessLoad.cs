namespace Squiggles.Core.Scenes.Utility;

using Godot;
using Squiggles.Core.Data;

/// <summary>
/// Forces the <see cref="Access"/> singleton to load settings from disk when this node is loaded in the scene tree
/// </summary>
[GlobalClass]
public partial class ForceAccessLoad : Node {
  public override void _Ready() {
    var _ = Access.FontOption; // forcing a load of the instance
  }
}
