namespace Squiggles.Core.Scenes.Utility.Camera;

using Godot;
using Squiggles.Core.Error;

public partial class VirtualCamera : Marker3D {
  [Export] private bool _pushCamOnReady = false;
  [Export] private bool _allowVCamChildren = false;

  [ExportGroup("Camera Lerping", "_Lerp")]
  [Export] public bool LerpCamera = true;
  [Export] private float _lerpDuration = 0.1f;

  [ExportGroup("Advanced Lerp Options", "_advLerp")]
  [Export] private bool _useAdvancedLerpOptions = false;
  [Export] private bool _advLerpPosX = true;
  [Export] private bool _advLerpPosY = true;
  [Export] private bool _advLerpPosZ = true;
  [Export] private bool _advLerpRotX = true;
  [Export] private bool _advLerpRotY = true;
  [Export] private bool _advLerpRotZ = true;

  public bool IsOnStack { get; private set; }

  public float LerpFactor => 1.0f / _lerpDuration;

  public override void _Ready() {
    if (_pushCamOnReady) {
      PushVCam();
    }

    if (GetChildCount() > 0 && !_allowVCamChildren) {
      Print.Warn("Removing VirtualCamera child nodes. These should be removed for release!");
      while (GetChildCount() > 0) {
        var child = GetChild(0);
        child.QueueFree();
        RemoveChild(child);
      }
    }
  }

  public void PushVCam() {
    GetBrain()?.PushCamera(this);
    IsOnStack = true;
  }

  public void PopVCam() {
    GetBrain()?.PopCamera(this);
    IsOnStack = false;
  }

  private CameraBrain GetBrain() {
    var brain = GetTree().GetFirstNodeInGroup("cam_brain") as CameraBrain;
    //Debugging.Assert(brain != null, "VirtualCamera failed to find CameraBrain in scene. Possibly it is missing??");
    return brain;
  }

  public override void _ExitTree() {
    var brain = GetBrain();
    if (brain == null) {
      return; // Brain has already been cleared
    }

    if (brain.HasCamera(this)) {
      PopVCam();
    }
  }

  public Transform3D GetNewTransform(Transform3D brainTransform, float delta) {
    if (!LerpCamera) {
      return GlobalTransform;
    }

    if (!_useAdvancedLerpOptions) {
      return brainTransform.InterpolateWith(GlobalTransform, LerpFactor * delta);
    }

    // Use Advanced Lerping Options
    var trans = brainTransform; // trans rights
    var factor = LerpFactor * delta;

    // Positions
    trans.Origin.X = _advLerpPosX ? Mathf.Lerp(trans.Origin.X, GlobalPosition.X, factor) : GlobalPosition.X;
    trans.Origin.Y = _advLerpPosY ? Mathf.Lerp(trans.Origin.Y, GlobalPosition.Y, factor) : GlobalPosition.Y;
    trans.Origin.Z = _advLerpPosZ ? Mathf.Lerp(trans.Origin.Z, GlobalPosition.Z, factor) : GlobalPosition.Z;

    // Rotations
    // TODO determine if this correctly lerps rotations
    trans.Basis.X = _advLerpRotX ? trans.Basis.X.Lerp(GlobalTransform.Basis.X, factor) : GlobalTransform.Basis.X;
    trans.Basis.Y = _advLerpRotY ? trans.Basis.Y.Lerp(GlobalTransform.Basis.Y, factor) : GlobalTransform.Basis.Y;
    trans.Basis.Z = _advLerpRotZ ? trans.Basis.Z.Lerp(GlobalTransform.Basis.Z, factor) : GlobalTransform.Basis.Z;
    return trans;
  }



}
