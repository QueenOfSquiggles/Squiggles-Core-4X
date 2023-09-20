namespace Squiggles.Core.Extension;

using Godot;
using static Godot.Tween;

public static class TweenExtensions {

  /// <summary>
  /// Sets the tween to use my preferred setting for a more bouncy effect. Standard builder pattern
  /// </summary>
  /// <param name="tween"></param>
  /// <returns></returns>
  public static Tween SetDefaultStyle(this Tween tween) => tween.SetEase(EaseType.InOut).SetTrans(TransitionType.Cubic);

}
