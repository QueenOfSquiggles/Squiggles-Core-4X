namespace Squiggles.Core.Scenes.Utility;

using Godot;
using Squiggles.Core.Data;

/// <summary>
/// Forces the <see cref="Graphics"/> settings to load when added to the scene tree
/// </summary>
[GlobalClass]
public partial class LoadGraphicsSettings : Node {

  public override void _Ready() {
    var _ = Graphics.SDFGI; // forces instance initialization
  }
}
