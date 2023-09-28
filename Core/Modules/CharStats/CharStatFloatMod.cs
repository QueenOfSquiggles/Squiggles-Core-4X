namespace Squiggles.Core.CharStats;
using Godot;

/// <summary>
/// A variant of <see cref="CharStatFloat"/> which intentionally removes itself after a given amount of time. Acting as a temporary modifier.
/// </summary>
[GlobalClass]
public partial class CharStatFloatMod : CharStatFloat {

  /// <summary>
  /// The duration which this stat will last (generally considered a buff/debuff but feel free to interpret otherwise)
  /// </summary>
  [Export] public float Duration = 1.0f;

  public override void _Ready() {
    base._Ready();
    DeathClock();
  }

  protected async void DeathClock() {
    var timer = GetTree().CreateTimer(Duration);
    await ToSignal(timer, "timeout");
    QueueFree();
  }
}
