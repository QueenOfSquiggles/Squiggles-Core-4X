using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class CameraBrain : Camera3D
{
    [Export(PropertyHint.Enum, "Process,Physics")] private int _UpdateMode = 0;
    private const int UPDATE_PROCESS = 0;
    private const int UPDATE_PHYSICS = 0;

    // treating this as a stack, but using list to let me remove elements anywhere
    private readonly List<VirtualCamera> _VCamStack = new();
    private VirtualCamera _CurrentTarget;

    public Vector2 Offset = new();

    //
    // API
    //

    public void PushCamera(VirtualCamera vcam)
    {
        _VCamStack.Insert(0, vcam);
    }

    public void PopCamera(VirtualCamera vcam)
    {
        _VCamStack.Remove(vcam);
    }

    public bool HasCamera(VirtualCamera vcam)
    {
        return _VCamStack.Contains(vcam);
    }

    //
    //  Background Systems
    //
    public override void _Ready()
    {
        TopLevel = true;
    }
    public override void _Process(double delta)
    {
        if (_UpdateMode != UPDATE_PROCESS) return;
        UpdateCamera((float)delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_UpdateMode != UPDATE_PHYSICS) return;
        UpdateCamera((float)delta);
    }

    private void UpdateCamera(float delta)
    {
        if (_VCamStack.Count <= 0) return;
        var target = _VCamStack[0];

        GlobalTransform = target.GetNewTransform(GlobalTransform, delta);

        // effectively treats the Offset as a new LocalPositon through basis transformation
        GlobalPosition += GlobalTransform.Basis.X * Offset.X;
        GlobalPosition += GlobalTransform.Basis.Y * Offset.Y;
    }


}
