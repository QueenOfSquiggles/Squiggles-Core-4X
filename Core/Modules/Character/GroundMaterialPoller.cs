namespace Squiggles.Core.Scenes.World;

using System.Threading.Tasks;
using Godot;
using Squiggles.Core.Error;
using Squiggles.Core.Extension;

/// <summary>
/// A component that polls for colliders that contain a <see cref="GroundMaterial"/> component (a GroundMaterial node is a direct child of the body, usually StaticBody3D).
/// </summary>
public partial class GroundMaterialPoller : RayCast3D {

  /// <summary>
  /// A signal the emits when the material changes
  /// </summary>
  /// <param name="ground_material">the new ground material found</param>
  [Signal] public delegate void OnNewMaterialFoundEventHandler(GroundMaterial ground_material);
  /// <summary>
  /// Which node group is searched to filter out extra mess.
  /// </summary>
  [Export] private string _groundMaterialGroup = "HasGroundMaterial";

  /// <summary>
  /// The currently found ground material
  /// </summary>
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
