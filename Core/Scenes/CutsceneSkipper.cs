using Godot;
using queen.extension;
using System;

public partial class CutsceneSkipper : Node
{

    [Export] private AnimationPlayer anim;

    private bool _IsListening = false;

    public override void _Input(InputEvent e)
    {
        if (!_IsListening) return;
        // any button input (unlikely to have noisy input) skips the cutscene
        if (e is InputEventKey or InputEventJoypadButton)
            SkipCutscene();
    }

    public void Start() => _IsListening = true;
    public void Stop() => _IsListening = false;

    private void SkipCutscene()
    {
        anim?.Seek(anim.CurrentAnimationLength, true);
    }

}
