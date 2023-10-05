namespace Squiggles.Core.Scenes.UI.Menus;

using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Error;
using Squiggles.Core.Events;
using Squiggles.Core.Extension;
using Squiggles.Core.Scenes.Utility;

/// <summary>
/// The play menu of the main menu. It shows a series of options but is only used when the game is configured to use multiple save slots.
/// </summary>
public partial class PlayMenu : PanelContainer {

  /// <summary>
  /// The root node to add new slots into
  /// </summary>
  [Export] private Control _slotsRoot;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() => LoadSlots();

  private void LoadSlots() {
    if (_slotsRoot is null) {
      return;
    }

    var slotInfo = SC4X.Config?.SaveSlotHandlingSettings?.SlotInfoProvider;
    Debugging.Assert(slotInfo is not null, "Failed to load slot info provider from play menu!");


    var slots = SaveData.GetKnownSaveSlots();
    _slotsRoot.RemoveAllChildren();
    foreach (var s in slots) {
      if (!s.StartsWith("Y")) {
        continue;
      }

      var btn = new Button();
      _slotsRoot.AddChild(btn);
      btn.Text = $"{slotInfo.GetSaveSlotName(s)}\n{slotInfo.GetSaveSlotSubtitle(s)}";
      btn.Pressed += () => LoadSlot(s);
    }
  }

  private void OnBtnContinue() {
    EventBus.Data.TriggerSerializeAll();
    SaveData.LoadMostRecentSaveSlot();
    if (GetTree().CurrentScene.SceneFilePath != GetLevelScene()) {
      SceneTransitions.LoadSceneAsync(GetLevelScene());
    }
    EventBus.Data.TriggerReload();
  }

  private void OnBtnNewGame() => LoadSlot(SaveData.CreateSaveSlotName()); // makes a new save slot for current time

  private void LoadSlot(string slot_name) {
    EventBus.Data.TriggerSerializeAll();
    SaveData.SetSaveSlot(slot_name);
    if (GetTree().CurrentScene.SceneFilePath != GetLevelScene()) {
      SceneTransitions.LoadSceneAsync(GetLevelScene());
    }

    EventBus.Data.TriggerReload();
  }

  private static string GetLevelScene() => SC4X.Config?.PlayScene ?? "";
}
