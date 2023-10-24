namespace Squiggles.Core.Events;

using System;
using Godot;
using Squiggles.Core.Attributes;


/// <summary>
/// The main Event Bus for SC4X. Some events are vestigial from the game I ripped the code out of. I'm slowly working on features as I go and this is definitely an area that could use some TLC
/// <para/>
/// A future refactor will likely remove many default events in favour of a registration system for custom events. Currently lower in priority, feel free to contribute improvements yourself or just wait until I get around to it.
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

/// <summary>
/// Events for handling and processing audio.
/// </summary>
[MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
public class EventsAudio {
  /// <summary>
  /// An event for handling when a sound is played in 3D space. Not used internally to SC4X (yet?)
  /// </summary>
  [MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
  public event Action<Vector3> OnAudioSpatial;
  /// <summary>
  /// An even for handling when any sound is played.
  /// </summary>
  [MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
  public event Action OnAudio;

  /// <summary>
  /// Triggers <see cref="OnAudioSpatial"/>
  /// </summary>
  /// <param name="position"></param>
  [MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
  public void TriggerOnAudioSpatial(Vector3 position) => OnAudioSpatial?.Invoke(position);
  /// <summary>
  /// Triggers <see cref="OnAudio"/>
  /// </summary>
  [MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
  public void TriggerOnAudio() => OnAudio?.Invoke();
}

public class EventsGameplay {
  /// <summary>
  /// Called when a level is loaded. (not used internally)
  /// </summary>
  public event Action OnLevelLoaded;
  /// <summary>
  /// Called when the game first starts (not used internally)
  /// </summary>
  public event Action OnGameStart;

  /// <summary>
  /// Called when the player "dies" if that is possible in the game. (Not used internally)
  /// </summary>
  public event Action OnPlayerDie;

  /// <summary>
  /// Called when the player "wins" if that is possible in the game. (Not used internally)
  /// </summary>
  public event Action OnPlayerWin;

  /// <summary>
  /// Called when a system is requesting the player is/isn't able to move. Used internally for things like cutscenes and GUI to prevent the player from moving during them.
  /// </summary>
  public event Action<bool> RequestPlayerAbleToMove;

  /// <summary>
  /// Called when the player's "stats" change: health, max health, energy, and max energy respectively.
  /// </summary>
  [MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
  public event Action<float, float, float, float> OnPlayerStatsUpdated;

  /// <summary>
  /// Called when the player's "money" changes
  /// </summary>
  [MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
  public event Action<int> PlayerMoneyChanged;

  /// <summary>
  /// Triggers <see cref="OnLevelLoaded"/>
  /// </summary>
  public void TriggerOnLevelLoaded() => OnLevelLoaded?.Invoke();
  /// <summary>
  /// Triggers <see cref="OnGameStart"/>
  /// </summary>
  public void TriggerOnGameStart() => OnGameStart?.Invoke();
  /// <summary>
  /// Triggers <see cref="OnPlayerDie"/>
  /// </summary>
  public void TriggerOnPlayerDie() => OnPlayerDie?.Invoke();
  /// <summary>
  /// Triggers <see cref="OnPlayerWin"/>
  /// </summary>
  public void TriggerOnPlayerWin() => OnPlayerWin?.Invoke();
  /// <summary>
  /// Triggers <see cref="RequestPlayerAbleToMove"/>
  /// </summary>
  public void TriggerRequestPlayerAbleToMove(bool can_move) => RequestPlayerAbleToMove?.Invoke(can_move);
  /// <summary>
  /// Triggers <see cref="OnPlayerStatsUpdated"/>
  /// </summary>
  [MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
  public void TriggerPlayerStatsUpdated(float health, float max_health, float energy, float max_energy) => OnPlayerStatsUpdated?.Invoke(health, max_health, energy, max_energy);
  /// <summary>
  /// Triggers <see cref="PlayerMoneyChanged"/>
  /// </summary>
  [MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
  public void TriggerPlayerMoneyChange(int new_total) => PlayerMoneyChanged?.Invoke(new_total);
}

public class EventsUI {
  /// <summary>
  /// Called when a generic GUI element is requested. <see cref="DefaultHUD"/> handles this event to display any generic GUI component requested.
  /// </summary>
  public event Action<Control> RequestGUI;
  /// <summary>
  /// Called to request a GUI close. This includes custom GUIs from <see cref="RequestGUI"/>. Handled by <see cref="DefaultHUD"/>
  /// </summary>
  public event Action RequestCloseGUI;
  /// <summary>
  /// Called to request a subtitle. Handled by <see cref="DefaultHUD"/>
  /// </summary>
  public event Action<string> RequestSubtitle;
  /// <summary>
  /// Called to request an alert pop-up. Handled by <see cref="DefaultHUD"/>
  /// </summary>
  public event Action<string> RequestAlert;
  /// <summary>
  /// Called to signal that the player is currently able to interact with something. Handled by <see cref="DefaultHUD"/>
  /// </summary>
  public event Action<string> MarkAbleToInteract;
  /// <summary>
  /// Called to signal that the player is no-longer able to interact with something. Handled by <see cref="DefaultHUD"/>
  /// </summary>
  public event Action MarkUnableToInteract;

  /// <summary>
  /// Called to request display of one or two inventories. The players and a secondary inventory respectively. Not currently used internally. Will be removed in future
  /// </summary>
  [MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
  public event Action<object, object> RequestInventory;

  /// <summary>
  ///Called to update the player inventory display. (item index, item id, item quantity)
  /// </summary>
  [MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
  public event Action<int, string, int> UpdatePlayerInventoryDisplay;

  /// <summary>
  /// Call to force selection of a specific index in the player inventory
  /// </summary>
  [MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
  public event Action<int> PlayerInventorySelectIndex;

  /// <summary>
  /// Called when the player's inventory size changes. (number of slots total)
  /// </summary>

  [MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
  public event Action<int> PlayerInventorySizeChange;

  /// <summary>
  /// Triggers <see cref="RequestGUI"/>
  /// </summary>
  public void TriggerRequestGUI(Control gui_node) => RequestGUI?.Invoke(gui_node);
  /// <summary>
  /// Triggers <see cref="RequestCloseGUI"/>
  /// </summary>
  public void TriggerRequestCloseGUI() => RequestCloseGUI?.Invoke();

  /// <summary>
  /// Triggers <see cref="RequestSubtitle"/>
  /// </summary>
  public void TriggerRequestSubtitle(string text) => RequestSubtitle?.Invoke(text);
  /// <summary>
  /// Triggers <see cref="RequestAlert"/>
  /// </summary>
  public void TriggerRequestAlert(string text) => RequestAlert?.Invoke(text);
  /// <summary>
  /// Triggers <see cref="MarkAbleToInteract"/>
  /// </summary>
  public void TriggerAbleToInteract(string text) => MarkAbleToInteract?.Invoke(text);
  /// <summary>
  /// Triggers <see cref="MarkUnableToInteract"/>
  /// </summary>
  public void TriggerUnableToInteract() => MarkUnableToInteract?.Invoke();
  /// <summary>
  /// Triggers <see cref="RequestInventory"/>
  /// </summary>
  public void TriggerRequestInventory(object playerContainer, object secondaryContainer = null) => RequestInventory?.Invoke(playerContainer, secondaryContainer);

  /// <summary>
  /// Triggers <see cref="UpdatePlayerInventoryDisplay"/>
  /// </summary>
  public void TriggerUpdatePlayeInventoryDisplay(int index, string item, int qty) => UpdatePlayerInventoryDisplay?.Invoke(index, item, qty);
  /// <summary>
  /// Triggers <see cref="PlayerInventorySelectIndex"/>
  /// </summary>
  public void TriggerPlayerInventorySelect(int index) => PlayerInventorySelectIndex?.Invoke(index);

  /// <summary>
  /// Triggers <see cref="PlayerInventorySizeChange"/>
  /// </summary>
  public void TriggerInventoryResized(int slots) => PlayerInventorySizeChange?.Invoke(slots);

}

/// <summary>
/// Future refactor will/should strip this section of the event bus in favour of a custom event registration system. So devs with needs not handled by this bus can still use a single event bus singleton for all their event needs
/// </summary>
[MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
public class EventsInventory {
  /// <summary>
  /// Called to add an item of (item id, quantity) to the player's inventory.
  /// </summary>
  [MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
  public event Action<string, int> GivePlayerItem;
  /// <summary>
  /// Called to consume an item from the player's inventory. (item id, quantity)
  /// </summary>
  [MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
  public event Action<string, int> ConsumePlayerItem;


  /// <summary>
  /// Triggers <see cref="GivePlayerItem"/>
  /// </summary>
  [MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
  public void TriggerGivePlayerItem(string itemkey, int count = 1) => GivePlayerItem?.Invoke(itemkey, count);
  /// <summary>
  /// Triggers <see cref="ConsumePlayerItem"/>
  /// </summary>
  [MarkForRefactor("Obselete", "Will be removed in future refactor. No current workaround")]
  public void TriggerConsumePlayerItem(string itemkey, int count = 1) => ConsumePlayerItem?.Invoke(itemkey, count);

}

public class EventsData {
  /// <summary>
  /// Called once the mods have been loaded for this game (if any mods are loaded)
  /// </summary>
  public event Action OnModsLoaded;

  /// <summary>
  /// Called to request that everything currently present in memory serializes itself to disk as per their individual needs.
  /// </summary>
  public event Action SerializeAll;
  /// <summary>
  /// Called to request that everything currently present in memory deserializes itself from disk per their individual needs.
  /// </summary>
  public event Action Reload;

  /// <summary>
  /// Triggers <see cref="Reload"/>
  /// </summary>
  public void TriggerReload() => Reload?.Invoke();
  /// <summary>
  /// Triggers <see cref="SerializeAll"/>
  /// </summary>
  public void TriggerSerializeAll() => SerializeAll?.Invoke();

  /// <summary>
  /// Triggers <see cref="OnModsLoaded"/>
  /// </summary>
  public void TriggerOnModsLoaded() => OnModsLoaded?.Invoke();
}
