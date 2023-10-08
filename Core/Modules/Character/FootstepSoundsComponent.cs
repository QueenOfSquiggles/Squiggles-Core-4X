namespace Squiggles.Core.Scenes.Character;

using Godot;
using Squiggles.Core.Error;
using Squiggles.Core.Scenes.World;

/// <summary>
/// A component designed for characters that will generate 3D sounds for footsteps. Uses <see cref="GroundMaterialPoller"/> to dynamically determine the appropriate set of SFX for the current ground material. If no <see cref="GroundMaterialPoller"/> is set, only the <see cref="_stepSound"/> stream will be used.
/// </summary>
public partial class FootstepSoundsComponent : Node3D {

  [Export] private AudioStream _defaultStepSound;
  [Export] private bool _doPolling;
  /// <summary>
  /// The minimum distance travelled before triggering a "step"
  /// </summary>
  [Export] private float _minDistanceStepSound = 1.0f;

  /// <summary>
  /// The minimum rotation amount before triggering a "step"
  /// </summary>
  [Export] private float _minRotationStepSound = Mathf.Pi * 0.5f;

  /// <summary>
  /// An <see cref="AudioStreamPlayer3D"/> that contains the step sound. This same player will be used to play all stepping sounds, so it should be placed near the character's feet.
  /// </summary>
  [ExportGroup("Node Paths")]
  [Export] private AudioStreamPlayer3D _stepSound;
  /// <summary>
  /// A reference to a <see cref="GroundMaterialPoller"/> component. If nothing is assigned, this component will not be able to detect changes in ground material. As such, it will only use a single step sound.
  /// </summary>
  [Export] private GroundMaterialPoller _groundPoller;



  /// <summary>
  /// Debugging field to display the change in translation
  /// </summary>
  [ExportGroup("Debugging")]
  [Export] private Vector3 _deltaMotion = Vector3.Zero;
  /// <summary>
  /// Debugging field to display the change in rotation
  /// </summary>
  [Export] private float _deltaRotation = 0f;

  /// <summary>
  /// The global position of the previous frame
  /// </summary>
  [Export] private Vector3 _lastPosition;
  /// <summary>
  /// The global rotation of the previous frame
  /// </summary>
  [Export] private float _lastRotation;
  /// <summary>
  /// The previous frame direction
  /// </summary>
  [Export] private float _lastDirection = 0;
  /// <summary>
  /// Whether the poller (<see cref="GroundMaterialPoller"/>) was colliding.
  /// </summary>
  private bool _lastPollerWasColliding;
  /// <summary>
  /// A mask multiplied to the raw motion vector to determine actual motion for purposes of determining steps. By default, this mask removes the Y component such that only XZ (horizontal) motion contributes to step sounds.
  /// </summary>
  private Vector3 _dMotionMask = new(1, 0, 1); // masks out y motion

  public override void _Ready() {
    if (_groundPoller is not null && _doPolling) {
      _groundPoller.OnNewMaterialFound += SetMaterialSound;
    }
    _stepSound.Stream = _defaultStepSound;
  }

  public override void _PhysicsProcess(double delta) {
    var dMotion = (GlobalPosition - _lastPosition) * _dMotionMask;
    var dRotation = GlobalRotation.Y - _lastRotation;
    var nColliding = _groundPoller?.IsColliding() ?? true; // if _groundPoller is null, consider always colliding so we always have step sounds.

    if (nColliding && !_lastPollerWasColliding) {
      // step on return to ground
      TryPlayStepSound();
      _lastDirection = Mathf.Sign(dRotation);
      _lastPosition = GlobalPosition;
      _lastRotation = GlobalRotation.Y;
      _lastPollerWasColliding = nColliding;
    }
    else
    if (!nColliding && _lastPollerWasColliding) {
      // on stop colliding, reset values
      ResetDeltas();
      _lastPollerWasColliding = nColliding;
      return; // exit early
    }
    else {
      _lastPollerWasColliding = nColliding;
      if (!nColliding) {
        return;
      }
    }

    _lastDirection = Mathf.Sign(dRotation);
    _lastPosition = GlobalPosition;
    _lastRotation = GlobalRotation.Y;

    _deltaMotion += dMotion;
    _deltaRotation += Mathf.Abs(dRotation); // adding abs so it's total rotation regardless of direction

    // Step from moving min distance
    if (_deltaMotion.Length() > _minDistanceStepSound) {
      TryPlayStepSound();
      ResetDeltas();
    }

    // Step from turning minimum amount
    if (_deltaRotation > _minRotationStepSound) {
      TryPlayStepSound();
      ResetDeltas();
    }
  }

  private void ResetDeltas() {
    _deltaMotion = Vector3.Zero;
    _deltaRotation = 0f;
  }

  private void TryPlayStepSound() {
    if (_stepSound is null) {
      Print.Warn("Trying to play step sound but audio stream was found null!");
      return;
    }
    _stepSound.Play();
  }


  private void SetMaterialSound(GroundMaterial ground_material) {
    if (_stepSound is null) {
      return;
    }

    var double_play = _stepSound.Playing;

    _stepSound.Stream = ground_material?.MaterialAudio ?? _defaultStepSound;
    if (double_play) {
      TryPlayStepSound();
    }
  }
}
