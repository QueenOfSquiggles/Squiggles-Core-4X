namespace Squiggles.Core.Scenes.UI.Menus;

using System.Threading.Tasks;
using Godot;
using Squiggles.Core.Events;
using Squiggles.Core.Extension;
using Squiggles.Core.Scenes.Utility;

public partial class PauseMenu : Control {

  [Export(PropertyHint.File, "*.tscn")] private string _mainMenuFile = "";
  [Export(PropertyHint.File, "*.tscn")] private string _optionsMenuFile = "";

  [Export] private Control _menuPanel;
  private Control _currentPopup;

  public override void _Ready() {
    GetTree().Paused = true;
    Input.MouseMode = Input.MouseModeEnum.Visible;
  }

  public override void _UnhandledInput(InputEvent @event) {
    if (@event.IsActionPressed("ui_cancel")) {

      ReturnToPlay();
      this.HandleInput();
    }
  }

  private async void ReturnToPlay() {
    EventBus.Data.TriggerSerializeAll();
    await Task.Delay(10);
    Input.MouseMode = Input.MouseModeEnum.Captured;
    GetTree().Paused = false;
    QueueFree();
  }

  private async void ExitToMainMenu() {
    EventBus.Data.TriggerSerializeAll();
    await Task.Delay(10);
    GetTree().Paused = false;
    SceneTransitions.LoadSceneAsync(_mainMenuFile);
  }

  private void OnBtnOptions() {
    if (_currentPopup is null || !IsInstanceValid(_currentPopup)) {
      CreateNewSlidingScene(_optionsMenuFile);
    }
    else {
      _currentPopup?.GetComponent<SlidingPanelComponent>()?.RemoveScene();
    }

  }
  private static void OnBtnSave() => EventBus.Data.TriggerSerializeAll();

  private static void OnBtnReloadLastSave() => EventBus.Data.TriggerReload();

  private void CreateNewSlidingScene(string scene_file) {
    var packed = GD.Load<PackedScene>(scene_file);
    var scene = packed?.Instantiate<Control>();
    if (scene is null) {
      return;
    }
    scene.GlobalPosition = new Vector2(_menuPanel?.Size.X ?? 0, 0);
    AddChild(scene);
    _currentPopup = scene;
  }
}
