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
    [Export] private float mouse_sensitivity = 0.003f;

    [ExportGroup("Movement")]
    [Export] private float speed = 5.0f;
    [Export] private float acceleration = 2.0f;
    [Export] private float deacceleration = 10.0f;


    [ExportGroup("Node Refs")]
    [Export] private NodePath cam_arm_path;
    private Node3D cam_arm;

    [Export] private NodePath anim_tree_path;
    private AnimationTree anim_tree;

    [Export] private NodePath raycast_path;
    private RayCast3D raycast;

    private Vector2 camera_look_vector = new();

    public override void _Ready()
    {
        this.GetNode(cam_arm_path, out cam_arm);
        this.GetNode(anim_tree_path, out anim_tree);
        this.GetNode(raycast_path, out raycast);

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
        if (Input.MouseMode != Input.MouseModeEnum.Captured) return;
        PerformMouseLook((float)delta);

        var input_dir = Input.GetVector("move_left", "move_right", "move_back", "move_forward");
        if (input_dir.LengthSquared() < 0.1f)
            input_dir = Input.GetVector("gamepad_move_left", "gamepad_move_right", "gamepad_move_back", "gamepad_move_forward");

        var intent_vec = new Vector3();
        intent_vec += cam_arm.GlobalTransform.Basis.X * input_dir.X;
        intent_vec += cam_arm.GlobalTransform.Basis.Z * input_dir.Y * -1;
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
        anim_tree.Set("parameters/MovementCycle/blend_position", intent_vec.Length());

        var accel = Mathf.Lerp(deacceleration, acceleration, intent_vec.Dot(Velocity.Normalized()) * 0.5f + 0.5f);
        if (!actively_moving) accel = deacceleration;

        Velocity = Velocity.Lerp(intent_vec * speed, accel * (float)delta);
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
        var look = (camera_look_vector.LengthSquared() > 0.1f) ?
            camera_look_vector :
            GetGamepadLookVector();
        look *= delta;

        RotateY(look.X * mouse_sensitivity);

        var rot = cam_arm.Rotation;
        rot.X += look.Y * mouse_sensitivity;
        var cl = Mathf.DegToRad(89.0f);
        rot.X = Mathf.Clamp(rot.X, -cl, cl);
        cam_arm.Rotation = rot;

        camera_look_vector = Vector2.Zero;
    }

    private Vector2 GAMEPAD_VEC_FLIP = new(-1, 1);
    private Vector2 GetGamepadLookVector()
    {
        return Input.GetVector("gamepad_look_left", "gamepad_look_right", "gamepad_look_down", "gamepad_look_up")
        * Controls.Instance.ControllerLookSensitivity
        * GAMEPAD_VEC_FLIP;
    }

    private bool was_colliding_interactable = false;
    private void CheckInteractionRay()
    {
        var collider = raycast.GetCollider();
        bool flag = collider is not null and IInteractable && (collider as IInteractable).IsActive();
        if (flag != was_colliding_interactable)
        {
            if (flag)
            {
                var item_name = (collider as IInteractable).GetActiveName();
                Events.GUI.TriggerAbleToInteract(item_name);
            }
            else
            {
                Events.GUI.TriggerUnableToInteract();
            }
        }
        was_colliding_interactable = flag;
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
        if (e is not InputEventMouseMotion) return false;
        var mm = e as InputEventMouseMotion;
        camera_look_vector += mm.Relative * Controls.Instance.MouseLookSensivity * -1f;
        return true;
    }

    private bool InputInteract(InputEvent e)
    {
        if (!e.IsActionPressed("interact")) return false;

        raycast.ForceRaycastUpdate();
        if (raycast.GetCollider() is Node collider && collider is IInteractable inter && inter.IsActive())
        {
            inter.Interact();
        }
        return true;
    }
}
