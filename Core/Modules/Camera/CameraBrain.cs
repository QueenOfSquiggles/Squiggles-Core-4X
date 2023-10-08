namespace Squiggles.Core.Scenes.Utility.Camera;

using System.Collections.Generic;
using Godot;

/// <summary>
/// The camera brain, inspired in name by my most used Unity addon "Cinemachine". A single camera brain is required in your 3D scene to allow <see cref="VirtualCamera"/>s to be detected. Virtual cameras are pushed and popped from a VCam Stack. The topmost active VCam is used to determine the camera brain position and the lerping settings on the vcam determine how snappy the camera brain is to that position. This allows for cutscenes and even small animations to push their own virtual camera (which is only slightly more expensive that just a Node3D) to override the previous camera. If you've used Unity and specifically Cinemachine, or really any other virtual camera system, you probably have plenty of ideas on how this could be used.
/// </summary>
[GlobalClass]
public partial class CameraBrain : Camera3D {
  /// <summary>
  /// The way this camera brain should update, whether on the process(0) or physics(1) step. Usually, Process is usually your best option. But the physics option will solve stutters if you are using a <see cref="VirtualCamera"/> that has lerping disabled and is moved during the physics step.
  /// </summary>
  [Export(PropertyHint.Enum, "Process,Physics")] private int _updateMode = 0;
  private const int UPDATE_PROCESS = 0;
  private const int UPDATE_PHYSICS = 1;

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
  public override void _Ready() {
    TopLevel = true;
    AddToGroup("cam_brain");
  }
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
