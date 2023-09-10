using Godot;
using System;

public partial class GroundMaterial : Node
{
    /// <summary>
    /// An audio stream for the material's footstep sounds. Ideally a randomizer to prevent repetition.
    /// </summary>
    [Export] public AudioStream MaterialAudio;

    /// <summary>
    /// A scalar for movement on this material. Must be polled to apply.
    /// </summary>
    [Export] public float MaterialDifficulty = 1.0f;
}
