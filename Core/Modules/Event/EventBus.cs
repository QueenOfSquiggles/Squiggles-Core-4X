namespace Squiggles.Core.Events;

using System;
using Godot;


/// <summary>
/// The main Event Bus for SC4X. Some events are vestigial from the game I ripped the code out of. I'm slowly working on features as I go and this is definitely an area that could use some TLC
/// </summary>
public static class EventBus {

  /// <summary>
  /// An instance of the <see cref="EventsAudio"/> class, which is a container for events that cleans up our event bus.
  /// </summary>
  public static EventsAudio Audio { get; private set; } = new();
  /// <summary>
  /// An instance of the <see cref="EventsGameplay"/> class, which is a container for events that cleans up our event bus.
  /// </summary>
  public static EventsGameplay Gameplay { get; private set; } = new();
  /// <summary>
  /// An instance of the <see cref="EventsUI"/> class, which is a container for events that cleans up our event bus.
  /// </summary>
  public static EventsUI GUI { get; private set; } = new();
  /// <summary>
  /// An instance of the <see cref="EventsInventory"/> class, which is a container for events that cleans up our event bus.
  /// </summary>
  public static EventsInventory Inventory { get; private set; } = new();
  /// <summary>
  /// An instance of the <see cref="EventsData"/> class, which is a container for events that cleans up our event bus.
  /// </summary>
  public static EventsData Data { get; private set; } = new();

}

public class EventsAudio {
  public event Action<Vector3> OnAudioSpatial;
  public event Action OnAudio;
  public void TriggerOnAudioSpatial(Vector3 position) => OnAudioSpatial?.Invoke(position);
  public void TriggerOnAudio() => OnAudio?.Invoke();
}

public class EventsGameplay {
  public event Action OnLevelLoaded;
  public event Action OnGameStart;
  public event Action OnPlayerDie;
  public event Action OnPlayerWin;
  public event Action<bool> RequestPlayerAbleToMove;
  public event Action<float, float, float, float> OnPlayerStatsUpdated;
  public event Action<int> PlayerMoneyChanged;


  public void TriggerOnLevelLoaded() => OnLevelLoaded?.Invoke();
  public void TriggerOnGameStart() => OnGameStart?.Invoke();
  public void TriggerOnPlayerDie() => OnPlayerDie?.Invoke();
  public void TriggerOnPlayerWin() => OnPlayerWin?.Invoke();
  public void TriggerRequestPlayerAbleToMove(bool can_move) => RequestPlayerAbleToMove?.Invoke(can_move);
  public void TriggerPlayerStatsUpdated(float health, float max_health, float energy, float max_energy) => OnPlayerStatsUpdated?.Invoke(health, max_health, energy, max_energy);

  public void TriggerPlayerMoneyChange(int new_total) => PlayerMoneyChanged?.Invoke(new_total);
}

public class EventsUI {
  public event Action<Control> RequestGUI;
  public event Action RequestCloseGUI;
  public event Action<string> RequestSubtitle;
  public event Action<string> RequestAlert;
  public event Action<string> MarkAbleToInteract;
  public event Action MarkUnableToInteract;
  public event Action<object, object> RequestInventory;
  public event Action<int, string, int> UpdatePlayerInventoryDisplay;
  public event Action<int> PlayerInventorySelectIndex;
  public event Action<int> PlayerInventorySizeChange;

  public void TriggerRequestGUI(Control gui_node) => RequestGUI?.Invoke(gui_node);
  public void TriggerRequestCloseGUI() => RequestCloseGUI?.Invoke();

  public void TriggerRequestSubtitle(string text) => RequestSubtitle?.Invoke(text);
  public void TriggerRequestAlert(string text) => RequestAlert?.Invoke(text);
  public void TriggerAbleToInteract(string text) => MarkAbleToInteract?.Invoke(text);
  public void TriggerUnableToInteract() => MarkUnableToInteract?.Invoke();
  public void TriggerRequestInventory(object playerContainer, object secondaryContainer = null) => RequestInventory?.Invoke(playerContainer, secondaryContainer);

  public void TriggerUpdatePlayeInventoryDisplay(int index, string item, int qty) => UpdatePlayerInventoryDisplay?.Invoke(index, item, qty);
  public void TriggerPlayerInventorySelect(int index) => PlayerInventorySelectIndex?.Invoke(index);

  public void TriggerInventoryResized(int slots) => PlayerInventorySizeChange?.Invoke(slots);

}

public class EventsInventory {
  public event Action<string, int> GivePlayerItem;
  public event Action<string, int> ConsumePlayerItem;


  public void TriggerGivePlayerItem(string itemkey, int count = 1) => GivePlayerItem?.Invoke(itemkey, count);
  public void TriggerConsumePlayerItem(string itemkey, int count = 1) => ConsumePlayerItem?.Invoke(itemkey, count);

}

public class EventsData {
  public event Action OnModsLoaded;

  public event Action SerializeAll;
  public event Action Reload;

  public void TriggerReload() => Reload?.Invoke();
  public void TriggerSerializeAll() => SerializeAll?.Invoke();

  public void TriggerOnModsLoaded() => OnModsLoaded?.Invoke();
}
