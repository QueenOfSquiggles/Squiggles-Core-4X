namespace Squiggles.Core.Scenes.UI.Menus;

using System.Threading.Tasks;
using Godot;
using Squiggles.Core.Error;
using Squiggles.Core.Events;
using Squiggles.Core.Extension;

/// <summary>
/// The root panel of the options menu. Basically redirects to the various categories of options available.
/// </summary>
public partial class OptionsMenu : Control {

  /// <summary>
  /// The root of the sliding scenes that are added.
  /// </summary>
  [Export] private Control _slidingSceneRoot;


  /// <summary>
  /// Path to the scene for the gameplay panel
  /// </summary>
  [ExportGroup("Panel Scene References")]
  [Export] private PackedScene _pathPanelGameplay;
  /// <summary>
  /// Path to the scene for the graphics panel
  /// </summary>
  [Export] private PackedScene _pathPanelGraphics;
  /// <summary>
  /// Path to the scene for the accessibility panel
  /// </summary>
  [Export] private PackedScene _pathPanelAccess;
  /// <summary>
  /// Path to the scene for the controls panel
  /// </summary>
  [Export] private PackedScene _pathPanelControls;
  /// <summary>
  /// Path to the scene for the audio panel
  /// </summary>
  [Export] private PackedScene _pathPanelAudio;


  private Node _currentPopup;
  private bool _isBusy;

  // TODO integrate a gameplay settings panel
  private void OnBtnGameplay() => DoPanelThing(_pathPanelGameplay);
  private void OnBtnGraphics() => DoPanelThing(_pathPanelGraphics);
  private void OnBtnAccess() => DoPanelThing(_pathPanelAccess);
  private void OnBtnAudio() => DoPanelThing(_pathPanelAudio);
  private void OnBtnControls() => DoPanelThing(_pathPanelControls);

  private async void DoPanelThing(PackedScene file_path) {
    if (_isBusy) {
      return;
    }

    EventBus.Data.TriggerSerializeAll();
    // Print.Debug($"OptionsMenu: attempting sliding in {file_path}");
    _isBusy = true;
    var result = await ClearOldSlidingScene();
    if (result) {
      CreateNewSlidingScene(file_path);
    }
    else {
      Print.Warn($"Options menu failed to create panel view for: {file_path?.ResourcePath}");
    }

    _isBusy = false;
  }

  private async Task<bool> ClearOldSlidingScene() {
    if (_currentPopup is null) {
      return true;
    }

    var sliding_comp = _currentPopup.GetComponent<SlidingPanelComponent>();
    if (sliding_comp is not null) {
      _ = sliding_comp.RemoveScene();
      await ToSignal(_currentPopup, "tree_exited");
    }
    return true;
  }

  private void CreateNewSlidingScene(PackedScene packed) {
    var scene = packed.Instantiate<Control>();
    if (scene is null) {
      return;
    }

    scene.GlobalPosition = new Vector2(Size.X, 0);
    _slidingSceneRoot.AddChild(scene);
    _currentPopup = scene;
  }

  public override void _ExitTree() => EventBus.Data.TriggerSerializeAll();

}
