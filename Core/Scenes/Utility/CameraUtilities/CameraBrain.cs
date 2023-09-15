namespace Squiggles.Core.Scenes.Utility.Camera;

using System.Collections.Generic;
using Godot;

public partial class CameraBrain : Camera3D {
  [Export(PropertyHint.Enum, "Process,Physics")] private int _updateMode = 0;
  private const int UPDATE_PROCESS = 0;
  private const int UPDATE_PHYSICS = 0;

  // treating this as a stack, but using list to let me remove elements anywhere
  private readonly List<VirtualCamera> _vCamStack = new();
  private VirtualCamera _currentTarget;

  public Vector2 Offset;

  //
  // API
  //

  public void PushCamera(VirtualCamera vcam) => _vCamStack.Insert(0, vcam);

  public void PopCamera(VirtualCamera vcam) => _vCamStack.Remove(vcam);

  public bool HasCamera(VirtualCamera vcam) => _vCamStack.Contains(vcam);

  //
  //  Background Systems
  //
  public override void _Ready() => TopLevel = true;
  public override void _Process(double delta) {
    if (_updateMode != UPDATE_PROCESS) {
      return;
    }

    UpdateCamera((float)delta);
  }

  public override void _PhysicsProcess(double delta) {
    if (_updateMode != UPDATE_PHYSICS) {
      return;
    }

    UpdateCamera((float)delta);
  }

  private void UpdateCamera(float delta) {
    if (_vCamStack.Count <= 0) {
      return;
    }

    var target = _vCamStack[0];

    GlobalTransform = target.GetNewTransform(GlobalTransform, delta);

    // effectively treats the Offset as a new LocalPositon through basis transformation
    GlobalPosition += GlobalTransform.Basis.X * Offset.X;
    GlobalPosition += GlobalTransform.Basis.Y * Offset.Y;
  }


}
