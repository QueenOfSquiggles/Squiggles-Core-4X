namespace Squiggles.Core.Scenes.UI.Menus;

using System.Threading.Tasks;
using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Error;
using Squiggles.Core.Events;
using Squiggles.Core.Extension;
using Squiggles.Core.Scenes.Utility;

public partial class MainMenu : Control {
  [Export(PropertyHint.File, "*.tscn")] private string _playMenuScene = "";
  [Export(PropertyHint.File, "*.tscn")] private string _options = "";
  [Export(PropertyHint.File, "*.tscn")] private string _creditsScene = "";

  [ExportGroup("Node Paths")]
  [Export] private Control _buttonsPanel;
  [Export] private TextureRect _gameLogo;
  [Export] private LinkButton _authorLabel;

  private Node _currentPopup;

  public override void _Ready() {
    Input.MouseMode = Input.MouseModeEnum.Visible;
    if (_gameLogo is not null) {
      _gameLogo.Texture = ThisIsYourMainScene.Config?.GameLogo;
    }

    if (_authorLabel is not null) {
      _authorLabel.Text = Tr(_authorLabel.Text).Replace("%s", ThisIsYourMainScene.Config?.AuthorName ?? "SET AUTHOR NAME IN CONFIG");
      _authorLabel.Uri = ThisIsYourMainScene.Config?.AuthorGamesURL ?? "";
    }
  }

  private async void OnBtnPlay() {
    if (SaveData.HasSaveData()) {
      if (_currentPopup is PlayMenu) {
        return;
      }

      await ClearOldSlidingScene();
      CreateNewSlidingScene(_playMenuScene);
    }
    else {
      SaveData.SetSaveSlot(SaveData.CreateSaveSlotName());
      EventBus.Data.TriggerSerializeAll(); // guarantees any open options menus save their data
      SceneTransitions.LoadSceneAsync(ThisIsYourMainScene.Config?.PlayScene ?? "");
    }
  }

  private static void OnBtnContinue() {
    SaveData.LoadMostRecentSaveSlot();
    EventBus.Data.TriggerSerializeAll(); // guarantees any open options menus save their data
    SceneTransitions.LoadSceneAsync(ThisIsYourMainScene.Config?.PlayScene ?? "");
  }

  private async void OnBtnOptions() {
    if (_currentPopup is OptionsMenu) {
      return;
    }

    await ClearOldSlidingScene();
    CreateNewSlidingScene(_options);
  }

  private async void OnBtnCredits() {
    if (_currentPopup is CreditsScene) {
      return;
    }

    await ClearOldSlidingScene();
    CreateNewSlidingScene(_creditsScene);
  }


  private async Task ClearOldSlidingScene() {
    if (_currentPopup is null || !IsInstanceValid(_currentPopup)) {
      return;
    }

    var sliding_comp = _currentPopup.GetComponent<SlidingPanelComponent>();
    if (sliding_comp is not null) {
      _ = sliding_comp.RemoveScene();
      await ToSignal(_currentPopup, "tree_exited");
    }
  }

  private void CreateNewSlidingScene(string scene_file) {
    var packed = GD.Load<PackedScene>(scene_file);
    var scene = packed.Instantiate<Control>();
    if (scene is null) {
      return;
    }

    scene.GlobalPosition = new Vector2(_buttonsPanel.Size.X, 0);
    AddChild(scene);
    _currentPopup = scene;
  }

  private void OnBtnQuit() {
    Print.Debug("Quitting game");
    GetTree().Quit();
  }


}
