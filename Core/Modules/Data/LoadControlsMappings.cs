namespace Squiggles.Core.Scenes.Utility;

using Godot;
using Squiggles.Core.Data;

/// <summary>
/// Forces the <see cref="Controls"/> to load instance data when this node is added to the scene tree
/// </summary>
[GlobalClass]
public partial class LoadControlsMappings : Node {

  // doesn't really have to be anything. Just forces the instance to be initialized, which involves loading from disk
  public override void _Ready() => Controls.GetCurrentMappingFor("ui_accept");
}
