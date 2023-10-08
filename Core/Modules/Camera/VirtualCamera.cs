namespace Squiggles.Core.Scenes.Utility.Camera;

using Godot;
using Squiggles.Core.Error;

/// <summary>
/// A virtual camera for use with <see cref="CameraBrain"/>. These are incredibly powerful little nuggets and I hope you love them as much as I do.
/// </summary>
/// <remarks>
/// vcams are just so fucking cool
/// </remarks>
[GlobalClass]
public partial class VirtualCamera : Marker3D {
  /// <summary>
  /// If true, the virtual camera pushes itself to the Camera Brain's stack as soon as this vcam is loaded into the scene.
  /// </summary>
  [Export] private bool _pushCamOnReady = false;
  /// <summary>
  /// Whether or not to allow child nodes of the virtual camera. Ideally, most vcams will not have any child nodes.
  /// </summary>
  [Export] private bool _allowVCamChildren = false;

  /// <summary>
  /// Determines whether or not to lerp the camera (if true the Camera Brain will smoothly transition to the position of the vcam. Else it will hard-snap to the transform)
  /// </summary>
  [ExportGroup("Camera Lerping", "_Lerp")]
  [Export] public bool LerpCamera = true;
  /// <summary>
  /// The duration (in seconds) that the CameraBrain lags behind the vcam. (if vcam does not move for that duration, the Camera Brain will come to rest at the same position)
  /// </summary>
  [Export] private float _lerpDuration = 0.1f;

  /// <summary>
  /// Toggles the advanced lerping functions which provide better customization, but create a performance hit.
  /// </summary>
  [ExportGroup("Advanced Lerp Options", "_advLerp")]
  [Export] private bool _useAdvancedLerpOptions = false;
  /// <summary>
  /// Toggles lerping the x-axis translations
  /// </summary>
  [Export] private bool _advLerpPosX = true;
  /// <summary>
  /// Toggles lerping the y-axis translations
  /// </summary>
  [Export] private bool _advLerpPosY = true;
  /// <summary>
  /// Toggles lerping the z-axis translations
  /// </summary>
  [Export] private bool _advLerpPosZ = true;
  /// <summary>
  /// Toggles lerping the x-axis rotations
  /// </summary>
  [Export] private bool _advLerpRotX = true;
  /// <summary>
  /// Toggles lerping the y-axis rotations
  /// </summary>
  [Export] private bool _advLerpRotY = true;
  /// <summary>
  /// Toggles lerping the z-axis rotations
  /// </summary>
  [Export] private bool _advLerpRotZ = true;

  /// <summary>
  /// Helpful for knowing if the vcam is currently on the stack or not.
  /// </summary>
  public bool IsOnStack { get; private set; }

  /// <summary>
  /// The lerp factor (calculated from <see cref="_lerpDuration"/>) to achieve the desired lerp duration
  /// </summary>
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

  /// <summary>
  /// Pushes this vcam to the CameraBrain stack
  /// </summary>
  public void PushVCam() {
    GetBrain()?.PushCamera(this);
    IsOnStack = true;
  }

  /// <summary>
  /// Pops (removes) this vcam from the CameraBrain stack
  /// </summary>
  public void PopVCam() {
    GetBrain()?.PopCamera(this);
    IsOnStack = false;
  }

  private CameraBrain GetBrain() {
    var brain = GetTree()?.GetFirstNodeInGroup("cam_brain") as CameraBrain;
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
