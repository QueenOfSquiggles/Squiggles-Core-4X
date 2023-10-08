namespace Squiggles.Core.Scenes.Character;

using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Events;
using Squiggles.Core.Interaction;

/// <summary>
/// A fairly rudimentary character controller for FPS style movement. Lots of exported variables to allow a variety of game feels.
/// </summary>
public partial class PlayerController : CharacterBody3D {
  /// <summary>
  /// A scalar for sensitivity that scales the mouse movement down to something more usable.
  /// </summary>
  [ExportGroup("Controls")]
  [Export] private float _mouseSensitivity = 0.003f;

  /// <summary>
  /// The base movement speed
  /// </summary>
  [ExportGroup("Movement")]
  [Export] private float _speed = 5.0f;
  /// <summary>
  /// The speed at which the player accelerates (lower values make the player feel almost lethargic)
  /// </summary>
  [Export] private float _acceleration = 2.0f;
  /// <summary>
  /// The speed at which the player decelerates (lower values feel slippery. higher values can feel too responsive to be realistic)
  /// </summary>
  [Export] private float _deacceleration = 10.0f;


  /// <summary>
  /// The "cam arm" node in the hierarchy
  /// </summary>
  [ExportGroup("Node Refs")]
  [Export] private Node3D _camArm;
  /// <summary>
  /// The animation tree for the player
  /// </summary>
  [Export] private AnimationTree _animTree;
  /// <summary>
  /// The raycast used for interaction detection
  /// </summary>
  [Export] private RayCast3D _rayCast;

  private Vector2 _cameraLookVector;

  public override void _Ready() {
    EventBus.Gameplay.RequestPlayerAbleToMove += HandleEventPlayerCanMove;
    Input.MouseMode = Input.MouseModeEnum.Captured;
  }

  public override void _ExitTree() => EventBus.Gameplay.RequestPlayerAbleToMove -= HandleEventPlayerCanMove;

  private void HandleEventPlayerCanMove(bool can_move) {
    SetPhysicsProcess(can_move);
    // prevents random motion after returning
    Velocity = Vector3.Zero;
    _cameraLookVector = Vector2.Zero;
  }

  public override void _PhysicsProcess(double delta) {
    if (_camArm is null || _animTree is null) {
      return;
    }

    if (Input.MouseMode != Input.MouseModeEnum.Captured) {
      return;
    }

    PerformMouseLook((float)delta);

    var input_dir = Input.GetVector("move_left", "move_right", "move_back", "move_forward");
    if (input_dir.LengthSquared() < 0.1f) {
      input_dir = Input.GetVector("gamepad_move_left", "gamepad_move_right", "gamepad_move_back", "gamepad_move_forward");
    }

    var intent_vec = new Vector3();
    intent_vec += _camArm.GlobalTransform.Basis.X * input_dir.X;
    intent_vec += _camArm.GlobalTransform.Basis.Z * input_dir.Y * -1;
    intent_vec.Y = 0f;

    var actively_moving = intent_vec.LengthSquared() > 0.1f;

    if (!actively_moving) {
      intent_vec = Vector3.Zero;
    }
    else {
      intent_vec.Y = 0f;
      intent_vec = intent_vec.Normalized();
    }
    _animTree.Set("parameters/MovementCycle/blend_position", intent_vec.Length());

    var accel = Mathf.Lerp(_deacceleration, _acceleration, (intent_vec.Dot(Velocity.Normalized()) * 0.5f) + 0.5f);
    if (!actively_moving) {
      accel = _deacceleration;
    }

    Velocity = Velocity.Lerp(intent_vec * _speed, accel * (float)delta);
    if (!IsOnFloor()) {
      var vel = Velocity;
      vel.Y = -9.8f;
      Velocity = vel;
    }
    MoveAndSlide();

    CheckInteractionRay();
  }

  private void PerformMouseLook(float delta) {
    if (_camArm is null) {
      return;
    }

    var look = (_cameraLookVector.LengthSquared() > 0.1f) ?
        _cameraLookVector :
        GetGamepadLookVector();
    look *= delta;

    RotateY(look.X * _mouseSensitivity);

    var rot = _camArm.Rotation;
    rot.X += look.Y * _mouseSensitivity;
    var cl = Mathf.DegToRad(89.0f);
    rot.X = Mathf.Clamp(rot.X, -cl, cl);
    _camArm.Rotation = rot;

    _cameraLookVector = Vector2.Zero;
  }

  private Vector2 _gamepadVecFlip = new(-1, 1);
  private Vector2 GetGamepadLookVector()
    => Input.GetVector("gamepad_look_left", "gamepad_look_right", "gamepad_look_down", "gamepad_look_up")
      * Controls.ControllerLookSensitivity
      * _gamepadVecFlip;

  private bool _wasCollidingInteractable;
  private void CheckInteractionRay() {
    if (_rayCast is null) {
      return;
    }

    var collider = _rayCast.GetCollider();

    var flag = false;
    var item_name = "";
    if (collider is IInteractable iis && iis.GetIsActive()) {
      item_name = iis.GetActiveName();
      flag = true;
    }

    if (flag != _wasCollidingInteractable) { // TODO there has GOT to be a better way  to lay this out
      if (flag) {
        EventBus.GUI.TriggerAbleToInteract(item_name);
      }
      else {
        EventBus.GUI.TriggerUnableToInteract();
      }
    }
    _wasCollidingInteractable = flag;
  }

  public override void _UnhandledInput(InputEvent @event) {
    var handled = false;
    if (Input.MouseMode == Input.MouseModeEnum.Captured) {
      handled = handled || InputMouseLook(@event);
      handled = handled || InputInteract(@event);
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

    _cameraLookVector += mm.Relative * Controls.MouseLookSensivity * -1f;
    return true;
  }

  private bool InputInteract(InputEvent e) {
    if (!e.IsActionPressed("interact")) {
      return false;
    }

    if (_rayCast is null) {
      return false;
    }

    _rayCast.ForceRaycastUpdate();
    if (_rayCast.GetCollider() is Node collider && collider is IInteractable inter && inter.GetIsActive()) {
      inter.Interact();
    }
    return true;
  }
}
