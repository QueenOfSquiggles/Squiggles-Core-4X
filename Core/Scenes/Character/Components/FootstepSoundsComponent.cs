using System;
using Godot;
using queen.error;
using queen.extension;

public partial class FootstepSoundsComponent : Node3D
{
    [Export] private float _MinDistanceStepSound = 1.0f;
    [Export] private float MinRotationStepSound = Mathf.Pi * 0.5f;

    [ExportGroup("Node Paths")]
    [Export] private AudioStreamPlayer3D StepSound;
    [Export] private GroundMaterialPoller _GroundPoller;



    [ExportGroup("Debugging")]
    [Export] private Vector3 _DeltaMotion = Vector3.Zero;
    [Export] private float _DeltaRotation = 0f;

    [Export] private Vector3 _LastPosition;
    [Export] private float _LastRotation;
    [Export] private float LastDirection = 0;
    private bool _LastPollerWasColliding = false;

    private Vector3 D_MOTION_MASK = new(1, 0, 1); // masks out y motion

    public override void _Ready()
    {
        if (_GroundPoller is null) return;
        _GroundPoller.OnNewMaterialFound += SetMaterialSound;
    }

    public override void _PhysicsProcess(double _delta)
    {
        var dMotion = (GlobalPosition - _LastPosition) * D_MOTION_MASK;
        var dRotation = GlobalRotation.Y - _LastRotation;
        var nColliding = _GroundPoller?.IsColliding() ?? false;

        if (nColliding && !_LastPollerWasColliding)
        {
            // step on return to ground
            TryPlayStepSound();
            LastDirection = Mathf.Sign(dRotation);
            _LastPosition = GlobalPosition;
            _LastRotation = GlobalRotation.Y;
            _LastPollerWasColliding = nColliding;
        }
        else
        if (!nColliding && _LastPollerWasColliding)
        {
            // on stop colliding, reset values
            ResetDeltas();
            _LastPollerWasColliding = nColliding;
            return; // exit early
        }
        else
        {
            _LastPollerWasColliding = nColliding;
            if (!nColliding) return;
        }

        LastDirection = Mathf.Sign(dRotation);
        _LastPosition = GlobalPosition;
        _LastRotation = GlobalRotation.Y;

        _DeltaMotion += dMotion;
        _DeltaRotation += Mathf.Abs(dRotation); // adding abs so it's total rotation regardless of direction

        // Step from moving min distance
        if (_DeltaMotion.Length() > _MinDistanceStepSound)
        {
            TryPlayStepSound();
            ResetDeltas();
        }

        // Step from turning minimum amount
        if (_DeltaRotation > MinRotationStepSound)
        {
            TryPlayStepSound();
            ResetDeltas();
        }
    }

    private void ResetDeltas()
    {
        _DeltaMotion = Vector3.Zero;
        _DeltaRotation = 0f;
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


    private void SetMaterialSound(GroundMaterial ground_material)
    {
        if (StepSound is null) return;
        var double_play = StepSound.Playing;

        StepSound.Stream = ground_material?.MaterialAudio;
        if (double_play) TryPlayStepSound();
    }
}
