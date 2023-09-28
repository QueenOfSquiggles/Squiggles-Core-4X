namespace Squiggles.Core.Scenes.Utility.Camera;

using Godot;
using Squiggles.Core.Error;

/// <summary>
/// A utility for the <see cref="CameraBrain"/> (or really any Camera3D if you like) that dynamically applies CameraAttributesPractical to the camera parent node and adjusts the depth of field effect to be the distance of the currently detected collision.
/// </summary>
[GlobalClass]
public partial class DynamicDOFRay : RayCast3D {

  /// <summary>
  /// Determines how fast (in seconds) the depth of field distance should lerp when changing (most noticeable when the distance changes drastically)
  /// </summary>
  [Export] private float _targetTransitionSpeed = 0.2f;
  /// <summary>
  /// The distance where objects are in focus surrounding the current point of focus.
  /// </summary>
  [Export] private float _transitionRangeMeters = 10.0f;

  /// <summary>
  /// The distance to lerp to when no collisions are detected.
  /// </summary>
  [Export] private float _defaultDistance = 50;
  private Camera3D _camera;
  private float _currentTargetDistance = float.MaxValue;

  private float _transitionSpeedLerpFactor = 0.1f;
  public override void _Ready() {
    _camera = GetParent() as Camera3D;
    if (_camera == null) {
      Print.Error($"{nameof(DynamicDOFRay)} requires a parent of type {nameof(Camera3D)}");
      QueueFree();
      return;
    }
    if (_camera.Attributes != null) {
      Print.Warn($"{nameof(DynamicDOFRay)} requires the parent camera leave the 'attributes' parameter as null. The value is reassigned from this class.");
    }
    _camera.Attributes = new CameraAttributesPractical() {
      DofBlurFarEnabled = true,
      DofBlurNearEnabled = true,

      DofBlurNearTransition = _transitionRangeMeters,
      DofBlurNearDistance = _defaultDistance,

      DofBlurFarTransition = _transitionRangeMeters,
      DofBlurFarDistance = _defaultDistance,
    };
    _transitionSpeedLerpFactor = 1.0f / _targetTransitionSpeed;
  }

  public override void _Process(double delta) {
    if (_camera?.Attributes is not CameraAttributesPractical att) {
      return;
    }

    att.DofBlurFarDistance = Mathf.Lerp(att.DofBlurFarDistance, _currentTargetDistance, ((float)delta) * _transitionSpeedLerpFactor);
    att.DofBlurNearDistance = Mathf.Lerp(att.DofBlurNearDistance, _currentTargetDistance, ((float)delta) * _transitionSpeedLerpFactor);
  }

  public override void _PhysicsProcess(double delta) {
    if (!IsColliding()) {
      _currentTargetDistance = _defaultDistance;
      return;
    }
    var delta_position = GetCollisionPoint() - GlobalPosition;
    _currentTargetDistance = delta_position.Length();
  }
}
