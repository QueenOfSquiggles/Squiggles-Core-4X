namespace Squiggles.Core.Scenes.Utility;

using Godot;
using Squiggles.Core.Data;

public partial class ForceAccessLoad : Node {
  public override void _Ready() {
    var _ = Access.Instance.FontOption; // forcing a load of the instance
  }
}
