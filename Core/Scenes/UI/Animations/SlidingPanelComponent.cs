using System;
using System.Threading.Tasks;
using Godot;
using queen.extension;

[GlobalClass]
public partial class SlidingPanelComponent : Node
{

    [Export] private float _PopInDuration = 0.3f;
    [Export] private float _PopOutDuration = 0.3f;
    [Export] private Control _Target;
    [Export] private NodePath _PathSubSlidingRoot = "..";
    [Export] private Tween.TransitionType _TransShow = Tween.TransitionType.Cubic;
    [Export] private Tween.TransitionType _TransHide = Tween.TransitionType.Cubic;
    [Export] private Tween.EaseType _EasingShow = Tween.EaseType.Out;
    [Export] private Tween.EaseType _EasingHide = Tween.EaseType.Out;

    private bool IsStable = false; // mood

    public override void _Ready()
    {
        _Target.ZIndex -= 1;
        float xStart = _Target.Position.X;
        _Target.Position -= new Vector2(_Target.Size.X, 0.0f);
        var tween = CreateTween().SetTrans(_TransShow).SetEase(_EasingShow);
        tween.TweenProperty(_Target, "position:x", xStart, _PopInDuration);
        tween.TweenProperty(this, nameof(IsStable), true, 0.01f);
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (!e.IsActionPressed("ui_cancel") || !IsStable) return;
        _ = RemoveScene();
    }

    public async Task RemoveScene()
    {
        if (!IsStable) return;
        if (_Target is null) return;
        this.GetSafe(_PathSubSlidingRoot, out Control SubSliders, false);
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
        var tween = CreateTween().SetTrans(_TransHide).SetEase(_EasingHide);
        tween.TweenProperty(_Target, "position:x", _Target.Position.X - _Target.Size.X, _PopOutDuration);
        tween.TweenCallback(Callable.From(_Target.QueueFree));
    }


}
