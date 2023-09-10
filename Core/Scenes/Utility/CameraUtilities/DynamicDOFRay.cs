using Godot;
using queen.error;
using System;

public partial class DynamicDOFRay : RayCast3D
{

    [Export] private float TargetTransitionSpeed = 0.2f;
    [Export] private float transition_range_meters = 10.0f;
    [Export] private float default_distance = 50;
    private Camera3D camera;
    private float current_target_distance = float.MaxValue;

    private float transition_speed_lerp_factor = 0.1f;
    public override void _Ready()
    {
        camera = GetParent() as Camera3D;
        if (camera == null)
        {
            Print.Error($"{nameof(DynamicDOFRay)} requires a parent of type {nameof(Camera3D)}");
            QueueFree();
            return;
        }
        if (camera.Attributes != null)
        {
            Print.Warn($"{nameof(DynamicDOFRay)} requires the parent camera leave the 'attributes' parameter as null. The value is reassigned from this class.");
        }
        camera.Attributes = new CameraAttributesPractical()
        {
            DofBlurFarEnabled = true,
            DofBlurNearEnabled = true,
            
            DofBlurNearTransition = transition_range_meters,
            DofBlurNearDistance = default_distance,
            
            DofBlurFarTransition = transition_range_meters,
            DofBlurFarDistance = default_distance,
        };
        transition_speed_lerp_factor = 1.0f / TargetTransitionSpeed;
    }

    public override void _Process(double delta)
    {
        var att = camera.Attributes as CameraAttributesPractical;
        att.DofBlurFarDistance = Mathf.Lerp(att.DofBlurFarDistance, current_target_distance, ((float)delta) * transition_speed_lerp_factor);
        att.DofBlurNearDistance = Mathf.Lerp(att.DofBlurNearDistance, current_target_distance, ((float)delta) * transition_speed_lerp_factor);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsColliding())
        {
            current_target_distance = default_distance;
            return;
        }
        var delta_position = GetCollisionPoint() - GlobalPosition;
        current_target_distance = delta_position.Length();
    }
}
