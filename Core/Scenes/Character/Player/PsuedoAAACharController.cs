namespace Squiggles.Core.Scenes.Character;

using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Interaction;
using Squiggles.Core.Events;
using Squiggles.Core.Error;
using Squiggles.Core.Scenes.Utility.Camera;
using Squiggles.Core.Attributes;

/// <summary>
/// The goal of this controller is to emulate the kind of controller that AAA horror games would use. Specifically targeting Resident Evil 7 (Biohazard). Whether or not is succeeds is up for debate.
/// <para/>
/// This class is designed to work out of the box, but has some key sections which allow extension. Generally, with things like this I want them to be able to be treated as a "black box", that is, a system with completely opaque internal operations. Read more about "blackboxing" here: <see href="https://en.wikipedia.org/wiki/Blackboxing"/>
/// </summary>
/// <remarks>
/// The more and more I look through this class, the more I see that it could (should?) be cleaned up with a few FSMs. Maybe even an HFSM for handling all the important stuff. Possibly in a future refactor.
/// </remarks>
[MarkForRefactor("FSM/HFSM Cleanup", "This class is incredibly heavy and could seriously benefit from a number of FSMs to clean up the logic and reduce the need to store state information for all the different systems inside.")]
public partial class PsuedoAAACharController : CharacterBody3D {
  /// <summary>
  /// The base speed of the player
  /// </summary>
  [ExportGroup("Movement Stats")]
  [Export] protected float _speed = 2.0f;
  /// <summary>
  /// The sprinting speed of the player
  /// </summary>
  [Export] protected float _sprintSpeed = 5.0f;
  /// <summary>
  /// The acceleration amount for the player
  /// </summary>
  [Export] protected float _acceleration = 0.3f;
  /// <summary>
  /// The velocity to apply for jumps (uses the project settings gravity)
  /// </summary>
  [Export] protected float _jumpVelocity = 4.5f;
  /// <summary>
  /// A scalar to reduce mouse sentitivty to a manageable level.
  /// </summary>
  [Export] protected float _mouseSensitivity = 0.03f;
  /// <summary>
  /// A scalar to apply to the current speed when crouched
  /// </summary>
  [Export] protected float _crouchSpeedScale = 0.45f;
  /// <summary>
  /// The highest height that the player can "step", determines whether a ledge can be simply stepped up or if it prevents movement.
  /// </summary>
  [Export] protected float _stepHeight = 0.4f;
  /// <summary>
  /// The amount of force to apply when stepping. This required serious tweaking to get mostly working so as long as you don't change too much about the character's gravity or this value, maybe best to leave it alone, yeah?
  /// </summary>
  [Export] protected float _stepStrength = 3.0f;

  /// <summary>
  /// The <see cref="VirtualCamera"/> of this player.
  /// </summary>
  [ExportGroup("Node Refs")]
  [Export] protected VirtualCamera _vcam;
  /// <summary>
  /// The animation player
  /// </summary>
  [Export] protected AnimationPlayer _anim;
  /// <summary>
  /// A upwards facing raycast from the crouch position. Used to determine whether there is enough space for the plater to stand up from crouching.
  /// </summary>
  [Export] protected RayCast3D _canStandCheck;
  /// <summary>
  /// A horizontal raycast used to check the max height for stepping.
  /// </summary>
  [Export] protected RayCast3D _canStepCheckTop;
  /// <summary>
  /// A horizontal raycast used to check for whether a ledge is present.
  /// </summary>
  [Export] protected RayCast3D _canStepCheckBottom;
  /// <summary>
  /// A raycast used to detect interactable objects
  /// </summary>
  [Export] protected RayCast3D _interactionRay;

  // Values
  /// <summary>
  /// The current frame camera movement vector. Composited between mouse and gamepad look values.
  /// </summary>
  protected Vector2 _camera_look_vector;
  /// <summary>
  /// The physics 3D gravity stored in the project settings
  /// </summary>
  protected float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
  /// <summary>
  /// The current speed, used for lerping from crouching/walking/sprinting
  /// </summary>
  protected float _currentSpeed;
  /// <summary>
  /// Whether or not the player is currently crouching
  /// </summary>
  protected bool _isCrouching;
  /// <summary>
  /// Whether or not the player believes itself to be on "stairs" (that is a steppable ledge)
  /// </summary>
  protected bool _isOnStairs;

  /// <summary>
  /// How far to cast out the top step check ray
  /// </summary>
  protected float _canStepCheckTopCastLength = 1.0f;
  /// <summary>
  /// How far to cast out the bottom step check ray
  /// </summary>
  private float _canStepCheckBottomCastLength = 1.0f;
  /// <summary>
  /// The XZ movement vector for this current frame, used to determine whether the
  /// </summary>
  protected Vector2 _inputVector;
  /// <summary>
  /// Whether the controller detected an interactable object last frame. Used to detect state changes
  /// </summary>
  protected bool _lastWasInteractable;


  public override void _Ready() {
    if (_canStepCheckTop is not null && _canStepCheckBottom is not null) {
      _canStepCheckTop.Position += new Vector3(0, _stepHeight, 0);
      _canStepCheckBottomCastLength = _canStepCheckBottom.TargetPosition.Length();
      _canStepCheckTopCastLength = _canStepCheckTop.TargetPosition.Length();
    }

    EventBus.Gameplay.RequestPlayerAbleToMove += HandleEventPlayerCanMove;
    Input.MouseMode = Input.MouseModeEnum.Captured;
  }


  public override void _PhysicsProcess(double delta) {

    var velocity = Velocity;
    if (!IsOnFloor()) {
      velocity.Y -= _gravity * (float)delta;
    }

    if (Input.MouseMode == Input.MouseModeEnum.Captured) {
      CamLookLogic(delta);
      CamMoveLogic(ref velocity, delta);
      if (!_isCrouching) {
        JumpLogic(ref velocity, delta);
      }

      StepLogic(ref velocity, delta);
    }

    Velocity = velocity;
    MoveAndSlide();
  }

  private void JumpLogic(ref Vector3 velocity, double delta) {
    // Add the gravity.

    // Handle Jump.
    if (Input.IsActionJustPressed("ui_accept") && IsOnFloor()) {
      velocity.Y = _jumpVelocity;
    }
  }

  private void CamMoveLogic(ref Vector3 velocity, double delta) {

    _inputVector = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
    if (_inputVector.LengthSquared() < 0.1f) {
      _inputVector = Input.GetVector("gamepad_move_left", "gamepad_move_right", "gamepad_move_forward", "gamepad_move_back");
    }

    var direction = (Transform.Basis * new Vector3(_inputVector.X, 0, _inputVector.Y)).Normalized();
    if (direction != Vector3.Zero) {
      // TODO Holy fuck this is some messy logic. This should really get cleaned up somehow. Maybe by having a series of contributing factors? IDK
      // Sprint or No Sprint
      var target_speed = (IsOnFloor() && Input.IsActionPressed("sprint")) ? _sprintSpeed : _speed;
      // Crouching so speed is slowed unless on stairs
      if (_isCrouching) {
        target_speed = _isOnStairs ? _speed : (_speed * _crouchSpeedScale);
      }

      _currentSpeed = Mathf.Lerp(_currentSpeed, target_speed, _acceleration);
      velocity.X = direction.X * _currentSpeed;
      velocity.Z = direction.Z * _currentSpeed;
    }
    else {
      _currentSpeed = Mathf.Lerp(_currentSpeed, 0, _acceleration);
      velocity.X = Mathf.MoveToward(Velocity.X, 0, _currentSpeed);
      velocity.Z = Mathf.MoveToward(Velocity.Z, 0, _currentSpeed);
    }
  }

  private void StepLogic(ref Vector3 velocity, double delta) {
    if (_inputVector.LengthSquared() < 0.8f || _canStepCheckBottom is null || _canStepCheckTop is null) {
      return;
    }

    var dir = new Vector3(_inputVector.X, 0, _inputVector.Y);
    _canStepCheckBottom.TargetPosition = dir * _canStepCheckBottomCastLength;
    _canStepCheckTop.TargetPosition = dir * _canStepCheckTopCastLength;

    if (!IsOnWall()) {
      _isOnStairs = false;
      return;
    }

    _isOnStairs = _canStepCheckBottom.IsColliding() && !_canStepCheckTop.IsColliding();

    if (_isOnStairs) {
      velocity.Y = _stepHeight * _stepStrength;
    }
  }

  private void CamLookLogic(double delta) {
    if (_vcam is null) {
      return;
    }

    var look = (_camera_look_vector.LengthSquared() > 0.1f) ? _camera_look_vector : GetGamepadLookVector();
    look *= (float)delta;

    RotateY(look.X * _mouseSensitivity);

    var rot = _vcam.Rotation;
    rot.X += look.Y * _mouseSensitivity;
    var cl = Mathf.DegToRad(89.0f);
    rot.X = Mathf.Clamp(rot.X, -cl, cl);
    _vcam.Rotation = rot;

    _camera_look_vector = Vector2.Zero;
  }

  private Vector2 _gamepadVecFlip = new(-1, 1);
  private Vector2 GetGamepadLookVector()
    => Input.GetVector("gamepad_look_left", "gamepad_look_right", "gamepad_look_down", "gamepad_look_up")
      * Controls.ControllerLookSensitivity
      * _gamepadVecFlip;


  private void HandleEventPlayerCanMove(bool can_move) {
    SetPhysicsProcess(can_move);
    // prevents random motion after returning
    Velocity = Vector3.Zero;
    _camera_look_vector = Vector2.Zero;
  }
  public override void _UnhandledInput(InputEvent @event) {
    var handled = false;
    if (Input.MouseMode == Input.MouseModeEnum.Captured) {
      handled |= InputMouseLook(@event);
      handled |= InputInteract(@event);
      handled |= ExtraInputEventHandling(@event);
      // TODO add in other controls here!

    }
    if (handled) {
      GetViewport().SetInputAsHandled();
    }
  }
  private bool InputMouseLook(InputEvent e) {
    if (e is not InputEventMouseMotion mm) {
      return false;
    }

    _camera_look_vector += mm.Relative * Controls.MouseLookSensivity * -1f;
    return true;
  }

  private bool InputInteract(InputEvent e) {
    if (_interactionRay is null) {
      return false;
    }

    _interactionRay.ForceRaycastUpdate();

    if (_interactionRay.GetCollider() is Node collider && collider is IInteractable inter && inter.GetIsActive()) {
      if (!_lastWasInteractable) {
        _lastWasInteractable = true;
        EventBus.GUI.TriggerAbleToInteract(inter.GetActiveName());
      }

      if (!e.IsActionPressed("interact")) {
        return false;
      }
      else if (inter.Interact()) {
        // TODO: do we want anything to happen on this end? Realistically the Interact object should handle SFX, VFX, etc...
      }
    }
    else if (_lastWasInteractable) {
      _lastWasInteractable = false;
      EventBus.GUI.TriggerUnableToInteract();
    }

    return true;
  }

  /// <summary>
  /// This method is marked as <c>virtual</c> to allow you to override it in your extension in order to add more functionality to a custom character controller.
  /// </summary>
  /// <param name="e">the input event currently being processed</param>
  /// <returns>true if the event should be marked as handled. False if not.</returns>
  protected virtual bool ExtraInputEventHandling(InputEvent e) {
    Print.Debug($"AAA Controller is firing '{nameof(ExtraInputEventHandling)}'");
    return false;
  }


}
