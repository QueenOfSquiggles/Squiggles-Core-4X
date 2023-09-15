namespace Squiggles.Core.Scenes.Utility;

using Godot;
using Squiggles.Core.Data;

public partial class LoadGraphicsSettings : Node {

  public override void _Ready() {
    var _ = Graphics.Instance.SDFGI; // forces instance initialization
  }
}
