namespace Squiggles.Core.Scenes.World;

using System.Threading.Tasks;
using Godot;
using Squiggles.Core.Error;
using Squiggles.Core.Extension;

public partial class GroundMaterialPoller : RayCast3D {

  [Signal] public delegate void OnNewMaterialFoundEventHandler(GroundMaterial ground_material);
  [Export] private string _groundMaterialGroup = "HasGroundMaterial";
  public GroundMaterial Material { get; private set; }

  public override void _Ready() => DelayedForceUpdate(50);

  private async void DelayedForceUpdate(int delay_millis) {
    await Task.Delay(delay_millis);
    ForceRaycastUpdate();
  }
  public override void _PhysicsProcess(double delta) {
    if (!IsColliding() || GetCollider() is not Node3D n3d) {
      return;
    }

    if (!n3d.IsInGroup(_groundMaterialGroup)) {
      Material = null;
      EmitSignal(nameof(OnNewMaterialFound), Material);
      return;
    }

    var gm = n3d.GetComponent<GroundMaterial>();
    if (Material != gm) {
      Print.Info("Found ground material");
      Material = gm;
      EmitSignal(nameof(OnNewMaterialFound), Material);
    }
  }


}
