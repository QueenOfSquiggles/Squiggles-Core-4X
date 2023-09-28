namespace Squiggles.Core.Events;

using Godot;
using Squiggles.Core.Error;

/// <summary>
/// An emitter for any event that takes no arguments. Events with arguments will need some actual work to call. But this lets us run event emitting from animators or signal callbacks.
/// </summary>
[GlobalClass]
public partial class EventEmitter : Node {

  [Export(PropertyHint.Enum, "OnAudio,OnGameStart,OnLevelLoaded,OnPlayerDie,OnPlayerWin,RequestCloseGUI")] private string _eventName = "";

  public void EmitEvent() {
    switch (_eventName) {
      case "OnAudio":
        EventBus.Audio.TriggerOnAudio();
        break;
      case "OnGameStart":
        EventBus.Gameplay.TriggerOnGameStart();
        break;
      case "OnLevelLoaded":
        EventBus.Gameplay.TriggerOnLevelLoaded();
        break;
      case "OnPlayerDie":
        EventBus.Gameplay.TriggerOnPlayerDie();
        break;
      case "OnPlayerWin":
        EventBus.Gameplay.TriggerOnPlayerWin();
        break;
      case "RequestCloseGUI":
        EventBus.GUI.TriggerRequestCloseGUI();
        break;
      default:
        Print.Warn($"You forgot to set an event on this event emitter!!\n\tScene Tree Path {GetPath()}");
        break;
    }
  }

}
