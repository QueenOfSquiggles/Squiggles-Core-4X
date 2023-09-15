namespace Squiggles.Core.Extension;

using Godot;

public static class AnimationPlayerExtensions {
  public static SignalAwaiter WaitForCurrentAnimEnd(this AnimationPlayer anim) => anim.ToSignal(anim, "animation_finished");
  public static SignalAwaiter WaitForAnimChange(this AnimationPlayer anim) => anim.ToSignal(anim, "animation_changed");
  public static SignalAwaiter WaitForAnimStart(this AnimationPlayer anim) => anim.ToSignal(anim, "animation_started");
}
