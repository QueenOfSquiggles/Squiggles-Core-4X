namespace Squiggles.Core.Editor;
using Godot;

[GlobalClass]
public partial class HelperEditorOnlyMesh3D : MeshInstance3D {
  public override void _Ready() => QueueFree();
}
