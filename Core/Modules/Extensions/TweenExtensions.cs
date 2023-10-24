namespace Squiggles.Core.Extension;

using Godot;
using static Godot.Tween;

/// <summary>
/// SC4X Godot.Tween extension(s)
/// </summary>
public static class TweenExtensions {

  /// <summary>
  /// Sets the tween to use my preferred setting for a more bouncy effect. Standard builder pattern
  /// </summary>
  /// <param name="tween"></param>
  /// <returns></returns>
  public static Tween SetSC4XStyle(this Tween tween) => tween.SetEase(EaseType.InOut).SetTrans(TransitionType.Cubic);

}
