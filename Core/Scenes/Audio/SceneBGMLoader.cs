namespace Squiggles.Core.Scenes.Audio;

using Godot;
using Squiggles.Core.Scenes.Utility;

public partial class SceneBGMLoader : Node {
  [Export] private AudioStream _musicTrack;
  [Export(PropertyHint.Range, "0.0,3.0")] private float _crossfade_duration = 1.0f;

  public override void _Ready() => BGM.QueueSong(_musicTrack, _crossfade_duration);

}
