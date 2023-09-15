namespace Squiggles.Core.Scenes.Character;

using Godot;
using Squiggles.Core.Error;
using Squiggles.Core.Scenes.World;

public partial class FootstepSoundsComponent : Node3D {
  [Export] private float _minDistanceStepSound = 1.0f;
  [Export] private float _minRotationStepSound = Mathf.Pi * 0.5f;

  [ExportGroup("Node Paths")]
  [Export] private AudioStreamPlayer3D _stepSound;
  [Export] private GroundMaterialPoller _groundPoller;



  [ExportGroup("Debugging")]
  [Export] private Vector3 _deltaMotion = Vector3.Zero;
  [Export] private float _deltaRotation = 0f;

  [Export] private Vector3 _lastPosition;
  [Export] private float _lastRotation;
  [Export] private float _lastDirection = 0;
  private bool _lastPollerWasColliding;

  private Vector3 _dMotionMask = new(1, 0, 1); // masks out y motion

  public override void _Ready() {
    if (_groundPoller is null) {
      return;
    }

    _groundPoller.OnNewMaterialFound += SetMaterialSound;
  }

  public override void _PhysicsProcess(double delta) {
    var dMotion = (GlobalPosition - _lastPosition) * _dMotionMask;
    var dRotation = GlobalRotation.Y - _lastRotation;
    var nColliding = _groundPoller?.IsColliding() ?? false;

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

    _stepSound.Stream = ground_material?.MaterialAudio;
    if (double_play) {
      TryPlayStepSound();
    }
  }
}
