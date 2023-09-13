using System;
using Godot;
using queen.error;
using queen.extension;

/// <summary>
/// This singleton needs to be in the autoloads in Godot.
/// It handles BackGround Music (BGM). It assumes only one track at a time will be playing. Any arbitrary stream can be assigned, and the cross-fade duration can be configured
/// See also: <seealso cref="QueueSong"/>
/// </summary>
public partial class BGM : Node
{

    [Export] private AudioStreamPlayer _BusA;
    [Export] private AudioStreamPlayer _BusB;
    private bool bus_a_active = false;

    private static BGM Instance;

    private const float DB_ON = 0.0f;
    private const float DB_OFF = -79.0f;

    public override void _Ready()
    {
        _ = this.EnsureSingleton(ref Instance);
    }

    /// <summary>
    /// Loads a new stream to be used as background music. The music does persist across scenes because this scene does not unload.
    /// Crossfa 
    /// </summary>
    /// <param name="stream">The audio stream to play as bgm. Ideally this should be a loop. This can be null to stop all music</param>
    /// <param name="duration">The time it takes to crossfade between the current song and the next song. Defaults to 1 second</param>
    public static void QueueSong(AudioStream stream, float duration = 1.0f, bool continue_duplicate = true)
    {
        Instance?.InternalQueueSong(stream, duration, continue_duplicate);
    }

    private void InternalQueueSong(AudioStream stream, float duration, bool continue_duplicate)
    {
        if (_BusA is null || _BusB is null) return;
        if (continue_duplicate)
        {
            bool skip = false;
            if (bus_a_active && _BusA.Stream == stream) skip = true;
            if ((!bus_a_active) && _BusB.Stream == stream) skip = true;
            if (skip)
            {
                return;
            }
        }
        if (bus_a_active)
        {
            SwapSongs(stream, _BusB, _BusA, duration);
            bus_a_active = false;
        }
        else
        {
            SwapSongs(stream, _BusA, _BusB, duration);
            bus_a_active = true;
        }
    }

    private void SwapSongs(AudioStream stream, AudioStreamPlayer begin, AudioStreamPlayer end, float duration)
    {
        begin.Stream = stream;
        var tween = GetTree().CreateTween().SetDefaultStyle();
        tween.SetParallel(); // all tweens now run in parallel
        tween.TweenProperty(begin, "volume_db", DB_ON, duration);
        tween.TweenProperty(end, "volume_db", DB_OFF, duration);
        if (begin.Stream != null) begin.Play();

    }

}
