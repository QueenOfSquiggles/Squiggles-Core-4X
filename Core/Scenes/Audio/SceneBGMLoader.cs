using System;
using Godot;
using queen.error;

public partial class SceneBGMLoader : Node
{
    [Export] private AudioStream music_track;
    [Export(PropertyHint.Range, "0.0,3.0")] private float crossfade_duration = 1.0f;
    public override void _Ready()
	{
        BGM.QueueSong(music_track, crossfade_duration);
    }

}
