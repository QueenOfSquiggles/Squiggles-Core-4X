using Godot;
using static Godot.Tween;

namespace queen.extension;

public static class TweenExtensions
{

    /// <summary>
    /// Sets the tween to use my preferred setting for a more bouncy effect. Standard builder pattern
    /// </summary>
    /// <param name="tween"></param>
    /// <returns></returns>
    public static Tween SetDefaultStyle(this Tween tween)
    {
        return tween.SetEase(EaseType.InOut).SetTrans(TransitionType.Cubic);
    }

}