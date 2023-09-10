using System;
using Godot;
using queen.error;
using queen.extension;

public partial class FootstepSoundsComponent : Node3D
{
    [Export] private float MinDistanceStepSound = 1.0f;
    [Export] private float MinRotationStepSound = Mathf.Pi * 0.5f;

    [ExportGroup("Node Paths")]
    [Export] private NodePath PathStepSound;
    [Export] private NodePath PathGroundPoller;


    private AudioStreamPlayer3D StepSound;
    private GroundMaterialPoller GroundPoller;

    [ExportGroup("Debugging")]
    [Export] private Vector3 DeltaMotion = Vector3.Zero;
    [Export] private float DeltaRotation = 0f;

    [Export] private Vector3 LastPosition;
    [Export] private float LastRotation;
    [Export] private float LastDirection = 0;
    private bool LastPollerWasColliding = false;

    private Vector3 D_MOTION_MASK = new(1, 0, 1); // masks out y motion

    public override void _Ready()
    {
        this.GetSafe(PathStepSound, out StepSound);
        this.GetSafe(PathGroundPoller, out GroundPoller);
        GroundPoller.OnNewMaterialFound += SetMaterialSound;
    }

    public override void _PhysicsProcess(double _delta)
    {
        var dMotion = (GlobalPosition - LastPosition) * D_MOTION_MASK;
        var dRotation = GlobalRotation.Y - LastRotation;
        var nColliding = GroundPoller.IsColliding();

        if (nColliding && !LastPollerWasColliding)
        {
            // step on return to ground
            TryPlayStepSound();
            LastDirection = Mathf.Sign(dRotation);
            LastPosition = GlobalPosition;
            LastRotation = GlobalRotation.Y;
            LastPollerWasColliding = nColliding;
        }
        else
        if (!nColliding && LastPollerWasColliding)
        {
            // on stop colliding, reset values
            ResetDeltas();
            LastPollerWasColliding = nColliding;
            return; // exit early
        }
        else
        {
            LastPollerWasColliding = nColliding;
            if (!nColliding) return;
        }

        LastDirection = Mathf.Sign(dRotation);
        LastPosition = GlobalPosition;
        LastRotation = GlobalRotation.Y;

        DeltaMotion += dMotion;
        DeltaRotation += Mathf.Abs(dRotation); // adding abs so it's total rotation regardless of direction

        // Step from moving min distance
        if (DeltaMotion.Length() > MinDistanceStepSound)
        {
            TryPlayStepSound();
            ResetDeltas();
        }

        // Step from turning minimum amount
        if (DeltaRotation > MinRotationStepSound)
        {
            TryPlayStepSound();
            ResetDeltas();
        }
    }

    private void ResetDeltas()
    {
        DeltaMotion = Vector3.Zero;
        DeltaRotation = 0f;
    }

    private void TryPlayStepSound()
    {
        if (StepSound is null)
        {
            Print.Warn("Trying to play step sound but audio stream was found null!");
            return;
        }
        StepSound.Play();
    }


    private void SetMaterialSound(GroundMaterial? ground_material)
    {
        var double_play = StepSound.Playing;

        StepSound.Stream = ground_material?.MaterialAudio;
        if (double_play) TryPlayStepSound();
    }

}
