using System;
using Godot;
using queen.error;

public partial class VirtualCamera : Marker3D
{
    [Export] private bool _PushCamOnReady = false;
    [Export] private bool _AllowVCamChildren = false;

    [ExportGroup("Camera Lerping", "_Lerp")]
    [Export] public bool _LerpCamera = true;
    [Export] private float _LerpDuration = 0.1f;

    [ExportGroup("Advanced Lerp Options", "_Adv_Lerp")]
    [Export] private bool _UseAdvancedLerpOptions = false;
    [Export] private bool _Adv_LerpPosX = true;
    [Export] private bool Adv_LerpPosY = true;
    [Export] private bool Adv_LerpPosZ = true;
    [Export] private bool _Adv_LerpRotX = true;
    [Export] private bool Adv_LerpRotY = true;
    [Export] private bool _Adv_LerpRotZ = true;

    public bool IsOnStack { get; private set; } = false;

    public float LerpFactor
    {
        get
        {
            return 1.0f / _LerpDuration;
        }
    }

    public override void _Ready()
    {
        if (_PushCamOnReady) PushVCam();
        if (GetChildCount() > 0 && !_AllowVCamChildren)
        {
            Print.Warn("Removing VirtualCamera child nodes. These should be removed for release!");
            while (GetChildCount() > 0)
            {
                var child = GetChild(0);
                child.QueueFree();
                RemoveChild(child);
            }
        }
    }

    public void PushVCam()
    {
        GetBrain()?.PushCamera(this);
        IsOnStack = true;
    }

    public void PopVCam()
    {
        GetBrain()?.PopCamera(this);
        IsOnStack = false;
    }

    private CameraBrain GetBrain()
    {
        var brain = GetTree().GetFirstNodeInGroup("cam_brain") as CameraBrain;
        //Debugging.Assert(brain != null, "VirtualCamera failed to find CameraBrain in scene. Possibly it is missing??");
        return brain;
    }

    public override void _ExitTree()
    {
        var brain = GetBrain();
        if (brain == null) return; // Brain has already been cleared
        if (brain.HasCamera(this)) PopVCam();
    }

    public Transform3D GetNewTransform(Transform3D brainTransform, float delta)
    {
        if (!_LerpCamera) return GlobalTransform;
        if (!_UseAdvancedLerpOptions) return brainTransform.InterpolateWith(GlobalTransform, LerpFactor * delta);

        // Use Advanced Lerping Options
        var trans = brainTransform; // trans rights
        var factor = LerpFactor * delta;

        // Positions
        if (_Adv_LerpPosX) trans.Origin.X = Mathf.Lerp(trans.Origin.X, GlobalPosition.X, factor);
        else trans.Origin.X = GlobalPosition.X;

        if (Adv_LerpPosY) trans.Origin.Y = Mathf.Lerp(trans.Origin.Y, GlobalPosition.Y, factor);
        else trans.Origin.Y = GlobalPosition.Y;

        if (Adv_LerpPosZ) trans.Origin.Z = Mathf.Lerp(trans.Origin.Z, GlobalPosition.Z, factor);
        else trans.Origin.Z = GlobalPosition.Z;

        // Rotations

        // TODO determine if this correctly lerps rotations
        if (_Adv_LerpRotX) trans.Basis.X = trans.Basis.X.Lerp(GlobalTransform.Basis.X, factor);
        else trans.Basis.X = GlobalTransform.Basis.X;

        if (Adv_LerpRotY) trans.Basis.Y = trans.Basis.Y.Lerp(GlobalTransform.Basis.Y, factor);
        else trans.Basis.Y = GlobalTransform.Basis.Y;

        if (_Adv_LerpRotZ) trans.Basis.Z = trans.Basis.Z.Lerp(GlobalTransform.Basis.Z, factor);
        else trans.Basis.Z = GlobalTransform.Basis.Z;


        return trans;
    }



}
