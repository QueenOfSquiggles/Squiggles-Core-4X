namespace Squiggles.Core.Scenes.UI.Menus;

using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Events;
using Squiggles.Core.Extension;
using Squiggles.Core.Scenes.Utility;

public partial class PlayMenu : PanelContainer {

  [Export] private Control _slotsRoot;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() => LoadSlots();

  private void LoadSlots() {
    if (_slotsRoot is null) {
      return;
    }

    var slots = SaveData.GetKnownSaveSlots();
    _slotsRoot.RemoveAllChildren();
    foreach (var s in slots) {
      if (!s.StartsWith("Y")) {
        continue;
      }

      var btn = new Button();
      _slotsRoot.AddChild(btn);
      var date = SaveData.ParseSaveSlotName(s);
      btn.Text = $"{date.ToShortDateString()} - {date.ToShortTimeString()}";
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

  private static string GetLevelScene() => ThisIsYourMainScene.Config?.PlayScene ?? "";
}
