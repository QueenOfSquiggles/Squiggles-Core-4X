namespace queen.events;

using Godot;
using queen.error;

/// An emitter for any event that takes no arguments. Events with arguments will need some actual work to call. But this lets us run event emitting from animators or signal callbacks.
public partial class EventEmitter : Node
{

    [Export(PropertyHint.Enum, "OnAudio,OnGameStart,OnLevelLoaded,OnPlayerDie,OnPlayerWin,RequestCloseGUI")] private string event_name = "";

    public void EmitEvent()
    {
        switch (event_name)
        {
            case "OnAudio":
                Events.Audio.TriggerOnAudio();
                break;
            case "OnGameStart":
                Events.Gameplay.TriggerOnGameStart();
                break;
            case "OnLevelLoaded":
                Events.Gameplay.TriggerOnLevelLoaded();
                break;
            case "OnPlayerDie":
                Events.Gameplay.TriggerOnPlayerDie();
                break;
            case "OnPlayerWin":
                Events.Gameplay.TriggerOnPlayerWin();
                break;
            case "RequestCloseGUI":
                Events.GUI.TriggerRequestCloseGUI();
                break;
            default:
                Print.Warn($"You forgot to set an event on this event emitter!!\n\tScene Tree Path {GetPath()}");
                break;
        }
    }

}
