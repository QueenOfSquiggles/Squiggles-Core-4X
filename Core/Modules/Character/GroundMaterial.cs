namespace Squiggles.Core.Scenes.World;

using Godot;

/// <summary>
/// A component for marking a ground to have a particular material.
/// </summary>
public partial class GroundMaterial : Node {
  /// <summary>
  /// An audio stream for the material's footstep sounds. Ideally a randomizer to prevent repetition.
  /// </summary>
  [Export] public AudioStream MaterialAudio;

  /// <summary>
  /// A scalar for movement on this material. Must be polled to apply.
  /// </summary>
  [Export] public float MaterialDifficulty = 1.0f;
}
