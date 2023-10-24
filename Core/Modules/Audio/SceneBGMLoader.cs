namespace Squiggles.Core.Scenes.Audio;

using Godot;
using Squiggles.Core.Scenes.Utility;

/// <summary>
/// A utility that plays a music track when loaded into the scene. Uses <see cref="BGM"/> for sone queuing.
/// </summary>
[GlobalClass]
public partial class SceneBGMLoader : Node {
  /// <summary>
  /// The track to play
  /// </summary>
  [Export] private AudioStream _musicTrack;
  /// <summary>
  /// The time to crossfade.If a music track is already playing they will do a linear crossfade over this duration of seconds.
  /// </summary>
  [Export(PropertyHint.Range, "0.0,3.0")] private float _crossfade_duration = 1.0f;

  public override void _Ready() => BGM.QueueSong(_musicTrack, _crossfade_duration);

}
