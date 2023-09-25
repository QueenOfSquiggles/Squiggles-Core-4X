namespace Squiggles.Core.Extension;

using Godot;

/// <summary>
/// SC4X Extension for AnimationPlayer
/// </summary>
public static class AnimationPlayerExtensions {
  /// <summary>
  /// Returns a "SignalAwaiter" for the animation player that awaits until the "animation_finished" signal is emitted. Useful because I always forget the exact name of the signals so this exists. Await the SignalAwaited returned in an async method to do some operation once the animation player has completed.
  /// </summary>
  /// <param name="anim">the AnimationPlayer instance this is called on</param>
  /// <returns>A SignalAwaiter for "animation_finished"</returns>
  public static SignalAwaiter WaitForCurrentAnimEnd(this AnimationPlayer anim) => anim.ToSignal(anim, "animation_finished");

  /// <summary>
  /// Returns a signal awaiter for the animation player
  /// </summary>
  /// <param name="anim"></param>
  /// <returns>a signal awaiter for "animation_changed"</returns>
  public static SignalAwaiter WaitForAnimChange(this AnimationPlayer anim) => anim.ToSignal(anim, "animation_changed");
  /// <summary>
  /// returns a signal awaiter for when a new animation starts.
  /// </summary>
  /// <param name="anim"></param>
  /// <returns>a signal awaiter for "animation_started</returns>
  public static SignalAwaiter WaitForAnimStart(this AnimationPlayer anim) => anim.ToSignal(anim, "animation_started");
}
