using System;
using System.Threading.Tasks;
using Godot;
using queen.extension;

[GlobalClass]
public partial class SlidingPanelComponent : Node
{

    [Export] private float PopInDuration = 0.3f;
    [Export] private float PopOutDuration = 0.3f;
    [Export] private NodePath PathTargetControl = "..";
    [Export] private NodePath PathSubSliding = "..";
    [Export] private Tween.TransitionType TransShow = Tween.TransitionType.Cubic;
    [Export] private Tween.TransitionType TransHide = Tween.TransitionType.Cubic;
    [Export] private Tween.EaseType EasingShow = Tween.EaseType.Out;
    [Export] private Tween.EaseType EasingHide = Tween.EaseType.Out;

    private bool IsStable = false; // mood
    private Control Target;

    public override void _Ready()
    {
        this.GetSafe(PathTargetControl, out Target);
        Target.ZIndex -= 1;
        float xStart = Target.Position.X;
        Target.Position -= new Vector2(Target.Size.X, 0.0f);
        var tween = CreateTween().SetTrans(TransShow).SetEase(EasingShow);
        tween.TweenProperty(Target, "position:x", xStart, PopInDuration);
        tween.TweenProperty(this, nameof(IsStable), true, 0.01f);
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (!e.IsActionPressed("ui_cancel") || !IsStable) return;
        RemoveScene();
    }

    public async Task RemoveScene()
    {
        if (!IsStable) return;
        this.GetSafe(PathSubSliding, out Control SubSliders, false);
        if (SubSliders is not null)
        {
            foreach (var c in SubSliders.GetChildren())
            {
                var c_comp = c.GetComponent<SlidingPanelComponent>();
                if (c_comp is null) continue;
                await c_comp.RemoveScene();
            }
        }

        IsStable = false;
        var tween = CreateTween().SetTrans(TransHide).SetEase(EasingHide);
        tween.TweenProperty(Target, "position:x", Target.Position.X - Target.Size.X, PopOutDuration);
        tween.TweenCallback(Callable.From(Target.QueueFree));
    }


}
