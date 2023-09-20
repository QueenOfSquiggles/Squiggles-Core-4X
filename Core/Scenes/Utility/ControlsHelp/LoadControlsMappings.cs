namespace Squiggles.Core.Scenes.Utility;

using Godot;
using Squiggles.Core.Data;

public partial class LoadControlsMappings : Node {

  // doesn't really have to be anything. Just forces the instance to be initialized, which involves loading from disk
  public override void _Ready() => Controls.Instance.GetCurrentMappingFor("ui_accept");
}
