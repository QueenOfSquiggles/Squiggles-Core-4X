using System;
using System.Threading.Tasks;
using Godot;
using queen.error;
using queen.extension;

public partial class GroundMaterialPoller : RayCast3D
{

    [Signal] public delegate void OnNewMaterialFoundEventHandler(GroundMaterial ground_material);
    [Export] private string GroundMaterialGroup = "HasGroundMaterial";
    public GroundMaterial? Material { get; private set; } = null;

    public override void _Ready() => DelayedForceUpdate(50);

    private async void DelayedForceUpdate(int delay_millis)
    {
        await Task.Delay(delay_millis);
        ForceRaycastUpdate();
    }

    public override void _PhysicsProcess(double _delta)
    {
        if (!IsColliding()) return;
        if (GetCollider() is not Node3D n3d) return;

        if (!n3d.IsInGroup(GroundMaterialGroup))
        {
            Material = null;
            EmitSignal(nameof(OnNewMaterialFound), Material);
            return;
        }

        var gm = n3d.GetComponent<GroundMaterial>();
        // Nullable because if there is no ground component on the ground group, we are OK with a null material
        if (Material != gm)
        {
            Print.Info("Found ground material");
            Material = gm;
            EmitSignal(nameof(OnNewMaterialFound), Material);
        }
    }
}
