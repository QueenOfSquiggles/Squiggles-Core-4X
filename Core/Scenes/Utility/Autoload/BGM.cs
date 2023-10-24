namespace Squiggles.Core.Scenes.Utility;

using Godot;
using Squiggles.Core.Extension;

/// <summary>
/// It handles BackGround Music (BGM). It assumes only one track at a time will be playing. Any arbitrary stream can be assigned, and the cross-fade duration can be configured
/// See also: <seealso cref="QueueSong"/>
/// </summary>
public partial class BGM : Node {

  [Export] private AudioStreamPlayer _busA;
  [Export] private AudioStreamPlayer _busB;
  private bool _busAActive;

  private static BGM _instance;

  private const float DB_ON = 0.0f;
  private const float DB_OFF = -79.0f;

  public override void _Ready() => _ = this.EnsureSingleton(ref _instance);

  /// <summary>
  /// Loads a new stream to be used as background music. The music does persist across scenes because this scene does not unload.
  /// Crossfa
  /// </summary>
  /// <param name="stream">The audio stream to play as bgm. Ideally this should be a loop. This can be null to stop all music</param>
  /// <param name="duration">The time it takes to crossfade between the current song and the next song. Defaults to 1 second</param>
  public static void QueueSong(AudioStream stream, float duration = 1.0f, bool continue_duplicate = true) => _instance?.InternalQueueSong(stream, duration, continue_duplicate);

  private void InternalQueueSong(AudioStream stream, float duration, bool continue_duplicate) {
    if (_busA is null || _busB is null) {
      return;
    }

    if (continue_duplicate) {
      var skip = false;
      if (_busAActive && _busA.Stream == stream) {
        skip = true;
      }

      if ((!_busAActive) && _busB.Stream == stream) {
        skip = true;
      }

      if (skip) {
        return;
      }
    }
    if (_busAActive) {
      SwapSongs(stream, _busB, _busA, duration);
      _busAActive = false;
    }
    else {
      SwapSongs(stream, _busA, _busB, duration);
      _busAActive = true;
    }
  }

  private void SwapSongs(AudioStream stream, AudioStreamPlayer begin, AudioStreamPlayer end, float duration) {
    begin.Stream = stream;
    var tween = GetTree().CreateTween().SetSC4XStyle();
    tween.SetParallel(); // all tweens now run in parallel
    tween.TweenProperty(begin, "volume_db", DB_ON, duration);
    tween.TweenProperty(end, "volume_db", DB_OFF, duration);
    if (begin.Stream != null) {
      begin.Play();
    }
  }

}
