namespace Squiggles.Core.Scenes.Utility;

using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Squiggles.Core.Error;
using Squiggles.Core.Extension;

/// <summary>
/// Handles all scene transitions while using a shader and texture rect to display a screen wipe effect. See <see cref="LoadSceneAsync"/> and <see cref="LoadSceneImmediate"/> for more details
/// </summary>
public partial class SceneTransitions : Node {

  public static SceneTransitions Instance => _instance;
  private static SceneTransitions _instance;

  [Export] private bool _testing = false;
  [Export] private AnimationPlayer _anim;
  [Export] private TextureRect _transitionTexture;
  [Export] private ProgressBar _progress;
  [Export] private Control _progressPanel;
  public override void _Ready() {
    if (!this.EnsureSingleton(ref _instance)) { return; }
    _progressPanel.Visible = false;
  }

  /// <summary>
  /// Loads a scene asynchronously. It assumes the scene file will be somewhat large (enough to produce a visible stutter). The screen wipe covers the screen, then the scene is loaded instantaneously. Once that is complete, the screen wipe reveals the newly loaded scene.
  /// </summary>
  /// <remarks>
  /// There are better ways to do this, as this doesn't support a loading screen at the moment. But for now this is plenty of functionality for the games that I make. If there is demand for it, I could add support for a loading bar style loading screen. Else contributions are appreciated!
  /// </remarks>
  /// <param name="file_path">the path to a scene file that will be loaded into</param>
  /// <param name="use_sub_threads">optionally use multiple threads to load. If this is true, there may be a stutter on the main thread, but it will load faster. By default it is false. Use it when you're loading up a fairly large scene. Try without first though since it uses a ton of system resources!</param>
  public static void LoadSceneAsync(string file_path, bool use_sub_threads = false, string colour = "black", bool showProgressBar = false) => Instance?.InternalLoadSceneAsync(file_path, use_sub_threads, colour, showProgressBar);
  private async void InternalLoadSceneAsync(string file_path, bool use_sub_threads = false, string colour = "black", bool showProgressBar = false) {
    (_instance._transitionTexture?.Material as ShaderMaterial)?.SetShaderParameter("bg_colour", Color.FromString(colour, Colors.Black));

    var err = ResourceLoader.LoadThreadedRequest(file_path, "PackedScene", use_sub_threads);
    if (err != Error.Ok) {
      Print.Error($"Failed to load the scene file at {file_path} with the threaded load");
      return;
    }

    if (_testing) {
      Print.Debug($"Starting load scene: {file_path}");
    }

    await FadeOut();

    _progressPanel.Visible = showProgressBar;
    var progressData = new Array();
    var currentProgress = 0f;
    do {
      await Task.Delay(10);
      ResourceLoader.LoadThreadedGetStatus(file_path, progressData);
      if (progressData.Count > 0) {
        currentProgress = progressData[0].AsSingle();
        _progress.Value = currentProgress;
      }
    } while (currentProgress < 1.0f);

    if (ResourceLoader.LoadThreadedGet(file_path) is not PackedScene scene) {
      Print.Error($"Failed to get scene resource. Path='{file_path}'");
      return;
    }
    _progressPanel.Visible = false;
    InternalLoadSceneImmediate(scene);
  }

  /// <summary>
  /// Unlike <see cref="LoadSceneAsync"/>, this immediately loads the specified scene. Notably without fading out first. However it does fade in upon completion.
  /// </summary>
  /// <param name="scene">the packed scene we want to load into</param>
  public static void LoadSceneImmediate(PackedScene scene) => Instance?.InternalLoadSceneImmediate(scene);
  private async void InternalLoadSceneImmediate(PackedScene scene) {
    if (scene is null) {
      return;
    }

    GetTree().ChangeSceneToPacked(scene);
    await FadeIn();
    if (_testing) {
      Print.Debug($"Finished loading scene: {scene.ResourcePath}");
    }
  }

  private SignalAwaiter FadeOut() {
    _anim.Play("FadeOut");
    return ToSignal(_anim, "animation_finished");
  }
  private SignalAwaiter FadeIn() {
    _anim.Play("FadeIn");
    return ToSignal(_anim, "animation_finished");
  }
}
