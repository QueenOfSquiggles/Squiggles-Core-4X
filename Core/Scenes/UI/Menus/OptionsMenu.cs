namespace Squiggles.Core.Scenes.UI.Menus;

using System.Threading.Tasks;
using Godot;
using Squiggles.Core.Attributes;
using Squiggles.Core.Error;
using Squiggles.Core.Events;
using Squiggles.Core.Extension;

/// <summary>
/// The root panel of the options menu. Basically redirects to the various categories of options available.
/// </summary>
public partial class OptionsMenu : Control {
  /// <summary>
  /// The main menu path. Legacy feature
  /// </summary>
  [MarkForRefactor("Legacy Feature", "")]
  [Export(PropertyHint.File, "*.tscn")] private string _mainMenuPath = "";
  /// <summary>
  /// The root of the sliding scenes that are added.
  /// </summary>
  [Export] private Control _slidingSceneRoot;


  /// <summary>
  /// Path to the scene for the gameplay panel
  /// </summary>
  [ExportGroup("Panel Scene References")]
  [Export(PropertyHint.File, "*.tscn")] private string _pathPanelGameplay = "";
  /// <summary>
  /// Path to the scene for the graphics panel
  /// </summary>
  [Export(PropertyHint.File, "*.tscn")] private string _pathPanelGraphics = "";
  /// <summary>
  /// Path to the scene for the accessibility panel
  /// </summary>
  [Export(PropertyHint.File, "*.tscn")] private string _pathPanelAccess = "";
  /// <summary>
  /// Path to the scene for the controls panel
  /// </summary>
  [Export(PropertyHint.File, "*.tscn")] private string _pathPanelControls = "";
  /// <summary>
  /// Path to the scene for the audio panel
  /// </summary>
  [Export(PropertyHint.File, "*.tscn")] private string _pathPanelAudio = "";


  private Node _currentPopup;
  private bool _isBusy;

  // TODO integrate a gameplay settings panel
  private void OnBtnGameplay() => DoPanelThing(_pathPanelGameplay);
  private void OnBtnGraphics() => DoPanelThing(_pathPanelGraphics);
  private void OnBtnAccess() => DoPanelThing(_pathPanelAccess);
  private void OnBtnAudio() => DoPanelThing(_pathPanelAudio);
  private void OnBtnControls() => DoPanelThing(_pathPanelControls);

  private async void DoPanelThing(string file_path) {
    if (_isBusy) {
      return;
    }

    EventBus.Data.TriggerSerializeAll();
    // Print.Debug($"OptionsMenu: attempting sliding in {file_path}");
    _isBusy = true;
    var result = await ClearOldSlidingScene(file_path);
    if (result) {
      CreateNewSlidingScene(file_path);
    }
    else {
      Print.Warn($"Options menu failed to create panel view for: {file_path.GetFile()}");
    }

    _isBusy = false;
  }

  private async Task<bool> ClearOldSlidingScene(string scene_file) {
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

  private void CreateNewSlidingScene(string scene_file) {
    var packed = GD.Load<PackedScene>(scene_file);
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
