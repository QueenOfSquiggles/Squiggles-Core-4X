namespace Squiggles.FSMTest;

using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Extension;
using Squiggles.Core.FSM;
using Squiggles.Core.Scenes.Utility.Camera;

/// <summary>
/// Utility class since the camera look logic isn't actually the interesting part of these states. child classes select how to handle when to transiton out
/// </summary>
public partial class PlayerCameraState : State {

  [Export] protected Node3D _camActor;
  [Export] protected Node3D _camArm;
  [Export] protected VirtualCamera _vcam;
  [Export] private float _max = 70.0f;
  [Export] private float _min = -70.0f;


  private const float MOUSE_SENSITIVITY = 0.003f;
  protected Vector2 _mouseMotion;

  public override void _Ready() => base._Ready();

  public override void EnterState() {
    SetPhysicsProcess(true);
    _vcam.PushVCam();
  }
  public override void ExitState() {
    SetPhysicsProcess(false);
    _vcam.PopVCam();
  }

  public override void _PhysicsProcess(double delta) {
    var look = (-_mouseMotion) * MOUSE_SENSITIVITY * ((float)delta) * Controls.Instance.MouseLookSensivity;
    _camActor.RotateY(look.X);

    var rot = _camArm.Rotation;
    rot.X = Mathf.Clamp(rot.X + look.Y, Mathf.DegToRad(_min), Mathf.DegToRad(_max));
    _camArm.Rotation = rot;


    _mouseMotion = Vector2.Zero;
  }

  public override void _UnhandledInput(InputEvent @event) {
    if (!IsActive) {
      return;
    }

    if (Input.MouseMode == Input.MouseModeEnum.Captured && @event is InputEventMouseMotion iemm) {
      _mouseMotion += iemm.Relative;
      this.HandleInput();
    }
  }


}
