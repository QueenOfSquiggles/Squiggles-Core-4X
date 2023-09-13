using System;
using Godot;
using interaction;
using queen.data;
using queen.error;
using queen.events;
using queen.extension;

public partial class PlayerController : CharacterBody3D
{
    [ExportGroup("Controls")]
    [Export] private float _MouseSensitivity = 0.003f;

    [ExportGroup("Movement")]
    [Export] private float _Speed = 5.0f;
    [Export] private float _Acceleration = 2.0f;
    [Export] private float _Deacceleration = 10.0f;


    [ExportGroup("Node Refs")]
    [Export] private Node3D _CamArm;
    [Export] private AnimationTree _AnimTree;
    [Export] private RayCast3D _RayCast;

    private Vector2 camera_look_vector = new();

    public override void _Ready()
    {
        Events.Gameplay.RequestPlayerAbleToMove += HandleEventPlayerCanMove;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _ExitTree()
    {
        Events.Gameplay.RequestPlayerAbleToMove -= HandleEventPlayerCanMove;
    }

    private void HandleEventPlayerCanMove(bool can_move)
    {
        SetPhysicsProcess(can_move);
        // prevents random motion after returning
        Velocity = Vector3.Zero;
        camera_look_vector = Vector2.Zero;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_CamArm is null || _AnimTree is null) return;
        if (Input.MouseMode != Input.MouseModeEnum.Captured) return;
        PerformMouseLook((float)delta);

        var input_dir = Input.GetVector("move_left", "move_right", "move_back", "move_forward");
        if (input_dir.LengthSquared() < 0.1f)
            input_dir = Input.GetVector("gamepad_move_left", "gamepad_move_right", "gamepad_move_back", "gamepad_move_forward");

        var intent_vec = new Vector3();
        intent_vec += _CamArm.GlobalTransform.Basis.X * input_dir.X;
        intent_vec += _CamArm.GlobalTransform.Basis.Z * input_dir.Y * -1;
        intent_vec.Y = 0f;

        bool actively_moving = intent_vec.LengthSquared() > 0.1f;

        if (!actively_moving)
        {
            intent_vec = Vector3.Zero;
        }
        else
        {
            intent_vec.Y = 0f;
            intent_vec = intent_vec.Normalized();
        }
        _AnimTree.Set("parameters/MovementCycle/blend_position", intent_vec.Length());

        var accel = Mathf.Lerp(_Deacceleration, _Acceleration, intent_vec.Dot(Velocity.Normalized()) * 0.5f + 0.5f);
        if (!actively_moving) accel = _Deacceleration;

        Velocity = Velocity.Lerp(intent_vec * _Speed, accel * (float)delta);
        if (!IsOnFloor())
        {
            var vel = Velocity;
            vel.Y = -9.8f;
            Velocity = vel;
        }
        MoveAndSlide();

        CheckInteractionRay();
    }

    private void PerformMouseLook(float delta)
    {
        if (_CamArm is null) return;
        var look = (camera_look_vector.LengthSquared() > 0.1f) ?
            camera_look_vector :
            GetGamepadLookVector();
        look *= delta;

        RotateY(look.X * _MouseSensitivity);

        var rot = _CamArm.Rotation;
        rot.X += look.Y * _MouseSensitivity;
        var cl = Mathf.DegToRad(89.0f);
        rot.X = Mathf.Clamp(rot.X, -cl, cl);
        _CamArm.Rotation = rot;

        camera_look_vector = Vector2.Zero;
    }

    private Vector2 GAMEPAD_VEC_FLIP = new(-1, 1);
    private Vector2 GetGamepadLookVector()
    {
        return Input.GetVector("gamepad_look_left", "gamepad_look_right", "gamepad_look_down", "gamepad_look_up")
        * Controls.Instance.ControllerLookSensitivity
        * GAMEPAD_VEC_FLIP;
    }

    private bool _WasCollidingInteractable = false;
    private void CheckInteractionRay()
    {
        if (_RayCast is null) return;
        var collider = _RayCast.GetCollider();

        bool flag = false;
        string item_name = "";
        if (collider is IInteractable iis && iis.IsActive())
        {
            item_name = iis.GetActiveName();
            flag = true;
        }

        if (flag != _WasCollidingInteractable)
        { // TODO there has GOT to be a better way  to lay this out
            if (flag) Events.GUI.TriggerAbleToInteract(item_name);
            else Events.GUI.TriggerUnableToInteract();
        }
        _WasCollidingInteractable = flag;
    }

    public override void _UnhandledInput(InputEvent e)
    {
        bool handled = false;
        if (Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            handled = handled || InputMouseLook(e);
            handled = handled || InputInteract(e);
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
        if (!e.IsActionPressed("interact")) return false;
        if (_RayCast is null) return false;

        _RayCast.ForceRaycastUpdate();
        if (_RayCast.GetCollider() is Node collider && collider is IInteractable inter && inter.IsActive())
        {
            inter.Interact();
        }
        return true;
    }
}
