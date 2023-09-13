using System;
using Godot;
using interaction;
using queen.data;
using queen.error;
using queen.events;
using queen.extension;

/// <summary>
/// The goal of this controller is to emulate the kind of controller that AAA horror games would use. Specifically targeting Resident Evil 7 (Biohazard). Whether or not is succeeds is up for debate.
/// </summary>
public partial class PsuedoAAACharController : CharacterBody3D
{
    [ExportGroup("Movement Stats")]
    [Export] protected float _Speed = 2.0f;
    [Export] protected float _SprintSpeed = 5.0f;
    [Export] protected float _Acceleration = 0.3f;
    [Export] protected float _JumpVelocity = 4.5f;
    [Export] protected float _MouseSensitivity = 0.03f;
    [Export] protected float _CrouchSpeedScale = 0.45f;
    [Export] protected float _StepHeight = 0.4f;
    [Export] protected float _StepStrength = 3.0f;

    [ExportGroup("Node Refs")]
    [Export] protected VirtualCamera _Vcam;
    [Export] protected AnimationPlayer _Anim;
    [Export] protected RayCast3D _CanStandCheck;
    [Export] protected RayCast3D _CanStepCheckTop;
    [Export] protected RayCast3D _CanStepCheckBottom;
    [Export] protected RayCast3D _InteractionRay;

    // Values
    protected Vector2 camera_look_vector = new();
    protected float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    protected float CurrentSpeed = 0.0f;
    protected bool IsCrouching = false;
    protected bool IsOnStairs = false;

    protected float CanStepCheckTop_CastLength = 1.0f;
    private float CanStepCheckBottom_CastLength = 1.0f;
    protected Vector2 InputVector = new();
    protected bool LastWasInteractable = false;

    public override void _Ready()
    {
        if (_CanStepCheckTop is not null && _CanStepCheckBottom is not null)
        {
            _CanStepCheckTop.Position += new Vector3(0, _StepHeight, 0);
            CanStepCheckBottom_CastLength = _CanStepCheckBottom.TargetPosition.Length();
            CanStepCheckTop_CastLength = _CanStepCheckTop.TargetPosition.Length();
        }

        Events.Gameplay.RequestPlayerAbleToMove += HandleEventPlayerCanMove;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }


    public override void _PhysicsProcess(double delta)
    {

        Vector3 velocity = Velocity;
        if (!IsOnFloor()) velocity.Y -= gravity * (float)delta;

        if (Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            CamLookLogic(delta);
            CamMoveLogic(ref velocity, delta);
            if (!IsCrouching) JumpLogic(ref velocity, delta);
            StepLogic(ref velocity, delta);
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    private void JumpLogic(ref Vector3 velocity, double delta)
    {
        // Add the gravity.

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
            velocity.Y = _JumpVelocity;
    }

    private void CamMoveLogic(ref Vector3 velocity, double delta)
    {

        InputVector = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        if (InputVector.LengthSquared() < 0.1f)
            InputVector = Input.GetVector("gamepad_move_left", "gamepad_move_right", "gamepad_move_forward", "gamepad_move_back");

        Vector3 direction = (Transform.Basis * new Vector3(InputVector.X, 0, InputVector.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            // TODO Holy fuck this is some messy logic. This should really get cleaned up somehow. Maybe by having a series of contributing factors? IDK
            // Sprint or No Sprint
            var target_speed = (IsOnFloor() && Input.IsActionPressed("sprint")) ? _SprintSpeed : _Speed;
            // Crouching so speed is slowed unless on stairs
            if (IsCrouching) target_speed = IsOnStairs ? _Speed : (_Speed * _CrouchSpeedScale);
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, target_speed, _Acceleration);
            velocity.X = direction.X * CurrentSpeed;
            velocity.Z = direction.Z * CurrentSpeed;
        }
        else
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, 0, _Acceleration);
            velocity.X = Mathf.MoveToward(Velocity.X, 0, CurrentSpeed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, CurrentSpeed);
        }
    }

    private void StepLogic(ref Vector3 velocity, double _delta)
    {
        if (InputVector.LengthSquared() < 0.8f) return;
        if (_CanStepCheckBottom is null || _CanStepCheckTop is null) return;

        var dir = new Vector3(InputVector.X, 0, InputVector.Y);
        _CanStepCheckBottom.TargetPosition = dir * CanStepCheckBottom_CastLength;
        _CanStepCheckTop.TargetPosition = dir * CanStepCheckTop_CastLength;

        if (!IsOnWall())
        {
            IsOnStairs = false;
            return;
        }

        IsOnStairs = _CanStepCheckBottom.IsColliding() && !_CanStepCheckTop.IsColliding();

        if (IsOnStairs)
        {
            velocity.Y = _StepHeight * _StepStrength;
        }
    }

    private void CamLookLogic(double delta)
    {
        if (_Vcam is null) return;
        var look = (camera_look_vector.LengthSquared() > 0.1f) ? camera_look_vector : GetGamepadLookVector();
        look *= (float)delta;

        RotateY(look.X * _MouseSensitivity);

        var rot = _Vcam.Rotation;
        rot.X += look.Y * _MouseSensitivity;
        var cl = Mathf.DegToRad(89.0f);
        rot.X = Mathf.Clamp(rot.X, -cl, cl);
        _Vcam.Rotation = rot;

        camera_look_vector = Vector2.Zero;
    }

    private Vector2 GAMEPAD_VEC_FLIP = new(-1, 1);
    private Vector2 GetGamepadLookVector()
    {
        return Input.GetVector("gamepad_look_left", "gamepad_look_right", "gamepad_look_down", "gamepad_look_up")
        * Controls.Instance.ControllerLookSensitivity
        * GAMEPAD_VEC_FLIP;
    }


    private void HandleEventPlayerCanMove(bool can_move)
    {
        SetPhysicsProcess(can_move);
        // prevents random motion after returning
        Velocity = Vector3.Zero;
        camera_look_vector = Vector2.Zero;
    }
    public override void _UnhandledInput(InputEvent e)
    {
        bool handled = false;
        if (Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            handled |= InputMouseLook(e);
            handled |= InputInteract(e);
            handled |= ExtraInputEventHandling(e);
            // TODO add in other controls here!

        }
        if (handled) GetViewport().SetInputAsHandled();
    }
    private bool InputMouseLook(InputEvent e)
    {
        if (e is not InputEventMouseMotion mm) return false;
        camera_look_vector += mm.Relative * Controls.Instance.MouseLookSensivity * -1f;
        return true;
    }

    private bool InputInteract(InputEvent e)
    {
        if (_InteractionRay is null) return false;
        _InteractionRay.ForceRaycastUpdate();

        if (_InteractionRay.GetCollider() is Node collider && collider is IInteractable inter && inter.IsActive())
        {
            if (!LastWasInteractable)
            {
                LastWasInteractable = true;
                Events.GUI.TriggerAbleToInteract(inter.GetActiveName());
            }

            if (!e.IsActionPressed("interact")) return false;
            else if (inter.Interact())
            {
                // TODO: do we want anything to happen on this end? Realistically the Interact object should handle SFX, VFX, etc...
            }
        }
        else if (LastWasInteractable)
        {
            LastWasInteractable = false;
            Events.GUI.TriggerUnableToInteract();
        }

        return true;
    }

    protected virtual bool ExtraInputEventHandling(InputEvent e)
    {
        Print.Debug($"AAA Controller is firing '{nameof(ExtraInputEventHandling)}'");
        return false;
    }


}
