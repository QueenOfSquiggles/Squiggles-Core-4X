namespace Squiggles.Core.Scenes.Utility;
using Godot;
using Squiggles.Core.Error;
using Squiggles.Core.Extension;

public partial class SceneTransitions : Node {

  public static SceneTransitions Instance => _instance;
  private static SceneTransitions _instance;

  [Export] private bool _testing = false;
  [Export] private AnimationPlayer _anim;
  public override void _Ready() {
    if (!this.EnsureSingleton(ref _instance)) { return; }
  }

  public static void LoadSceneAsync(string file_path, bool use_sub_threads = false) => Instance?.InternalLoadSceneAsync(file_path, use_sub_threads);
  private async void InternalLoadSceneAsync(string file_path, bool use_sub_threads = false) {
    var err = ResourceLoader.LoadThreadedRequest(file_path, "PackedScene", use_sub_threads);
    if (err != Error.Ok) {
      Print.Error($"Failed to load the scene file at {file_path} with the threaded load");
      return;
    }

    if (_testing) {
      Print.Debug($"Starting load scene: {file_path}");
    }

    await FadeOut();


    if (ResourceLoader.LoadThreadedGet(file_path) is not PackedScene scene) {
      Print.Error($"Failed to get scene resource. Path='{file_path}'");
      return;
    }

    InternalLoadSceneImmediate(scene);
  }

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
