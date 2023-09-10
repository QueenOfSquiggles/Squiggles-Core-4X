namespace queen.extension;

using Godot;

public static class AnimationPlayerExtensions
{
    public static SignalAwaiter WaitForCurrentAnimEnd(this AnimationPlayer anim)
    {
        return anim.ToSignal(anim, "animation_finished");
    }
    public static SignalAwaiter WaitForAnimChange(this AnimationPlayer anim)
    {
        return anim.ToSignal(anim, "animation_changed");
    }
    public static SignalAwaiter WaitForAnimStart(this AnimationPlayer anim)
    {
        return anim.ToSignal(anim, "animation_started");
    }
}