namespace Squiggles.Core.Editor;
using Godot;

/// <summary>
/// A utility class the simply Queues Free the mesh instance when <see cref="_Ready"/> is called. Maybe kinda dumb IDK if anyone else would use this LOL
/// </summary>
[GlobalClass]
public partial class HelperEditorOnlyMesh3D : MeshInstance3D {
  public override void _Ready() => QueueFree();
}
