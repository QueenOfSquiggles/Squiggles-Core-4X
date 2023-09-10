using Godot;
using queen.extension;
using System;

public partial class CutsceneSkipper : Node
{

    [Export] private NodePath path_animation_player;
    private AnimationPlayer anim;

    private bool IsListening = false;


    public override void _Ready()
    {
        this.GetNode(path_animation_player, out anim);
    }

    public override void _Input(InputEvent e)
    {
        if (!IsListening) return;
        // any button input (unlikely to have noisy input) skips the cutscene
        if (e is InputEventKey or InputEventJoypadButton)
            SkipCutscene();
    }

    public void Start() => IsListening = true;
    public void Stop() => IsListening = false;

    private void SkipCutscene()
    {
        anim.Seek(anim.CurrentAnimationLength, true);
    }

}
