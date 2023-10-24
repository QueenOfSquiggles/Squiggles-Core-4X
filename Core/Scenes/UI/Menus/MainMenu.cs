namespace Squiggles.Core.Scenes.UI.Menus;

using System.Threading.Tasks;
using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Error;
using Squiggles.Core.Events;
using Squiggles.Core.Extension;
using Squiggles.Core.Scenes.Utility;

/// <summary>
/// The main menu. Similar to <see cref="OptionsMenu"/>, it also serves mostly to redirect to other panes.
/// </summary>
public partial class MainMenu : Control {
  /// <summary>
  /// The play menu scene
  /// </summary>
  [Export] private PackedScene _playMenuScene;
  /// <summary>
  /// The options panel scene
  /// </summary>
  [Export] private PackedScene _options;
  /// <summary>
  /// the credits panel scene
  /// </summary>
  [Export] private PackedScene _creditsScene;

  /// <summary>
  /// the panel which contains the buttons
  /// </summary>
  [ExportGroup("Node Paths")]
  [Export] private Control _buttonsPanel;
  /// <summary>
  /// The texture of the game label
  /// </summary>
  [Export] private TextureRect _gameLogo;
  /// <summary>
  /// The link button that holds the author name and author link
  /// </summary>
  [Export] private LinkButton _authorLabel;
  [Export] private Button _continueButton;

  private Node _currentPopup;

  public override void _Ready() {
    Input.MouseMode = Input.MouseModeEnum.Visible;
    if (_gameLogo is not null) {
      _gameLogo.Texture = SC4X.Config?.GameLogo;
    }

    if (_authorLabel is not null) {
      _authorLabel.Text = Tr(_authorLabel.Text).Replace("%s", SC4X.Config?.AuthorName ?? "SET AUTHOR NAME IN CONFIG");
      _authorLabel.Uri = SC4X.Config?.AuthorGamesURL ?? "";
    }
    if (!SaveData.HasSaveData()) {
      _continueButton?.QueueFree();
    }
  }

  private async void OnBtnPlay() {
    switch (SC4X.Config?.SaveSlotHandlingSettings?.SlotOptions) {
      case Meta.SaveSlotSettings.SaveSlotOptions.NO_SAVE_DATA:
        SaveData.CurrentSaveSlot.DeleteSaveSlot(); // clear out any existing data
        SceneTransitions.LoadSceneAsync(SC4X.Config?.PlayScene ?? "");
        break;
      case Meta.SaveSlotSettings.SaveSlotOptions.SINGLE_SAVE_SLOT:
        SaveData.LoadDefaultSaveSlot();
        EventBus.Data.TriggerSerializeAll(); // guarantees any open options menus save their data
        SceneTransitions.LoadSceneAsync(SC4X.Config?.PlayScene ?? "");
        break;
      case Meta.SaveSlotSettings.SaveSlotOptions.MULTI_SLOT_SAVE_DATA:
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
          SceneTransitions.LoadSceneAsync(SC4X.Config?.PlayScene ?? "");
        }
        break;
      case null:
      default:
        Print.Error("Failed to handle main menu play pressing. Failed to find a valid save slot handling configuration", this);
        break;
    }
  }

  private void OnBtnContinue() {
    SaveData.LoadMostRecentSaveSlot();
    EventBus.Data.TriggerSerializeAll(); // guarantees any open options menus save their data
    SceneTransitions.LoadSceneAsync(SC4X.Config?.PlayScene ?? "", showProgressBar: true);
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

  private void CreateNewSlidingScene(PackedScene packed) {
    var scene = packed.Instantiate<Control>();
    if (scene is null) {
      return;
    }

    scene.GlobalPosition = new Vector2(_buttonsPanel.Size.X, 0);
    AddChild(scene);
    _currentPopup = scene;
  }

  private async void OnBtnQuit() {
    Print.Debug("Quitting game");
    await Task.Run(EventBus.Data.TriggerSerializeAll);
    GetTree().Quit();
  }

}
