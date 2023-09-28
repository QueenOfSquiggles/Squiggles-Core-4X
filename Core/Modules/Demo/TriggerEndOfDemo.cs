namespace Squiggles.Core.Scenes.Demo;
using Godot;
using Squiggles.Core.Error;
using Squiggles.Core.Scenes.Utility;

/// <summary>
/// A utility that can be called from a build regardless of whether the build is a demo build. It detects whether we are in a demo build context and then ends the game there. Useful for lazy people like me how want to have a demo, but don't want to have to manage an entirely different project folder.
/// </summary>
[GlobalClass]
public partial class TriggerEndOfDemo : Node {
  /// <summary>
  /// A reference to the scene file for "DemoEndScene" There is a default option <see cref="DemoEndScene"/> however, a custom end of demo scene would probably be for the best.
  /// </summary>
  [Export(PropertyHint.File, "*.tscn")] private string _demo_end_scene = "";

  /// <summary>
  /// Calling this method will do one of two things:
  /// <para/>
  /// <list type="bullet">
  /// <item>In a "demo" context it will load the specified demo end scene asynchronously.</item>
  /// <item>Outside of a "demo" context it will do nothing besides printing out an informational message.</item>
  /// </list>
  /// </summary>
  /// <remarks>
  /// Read more about feature tags in godot: <see href="https://docs.godotengine.org/en/stable/tutorials/export/feature_tags.html"/>
  /// </remarks>
  public void EndTheDemo() {
    if (OS.HasFeature("demo")) {
      Print.Info("Demo has completed. Ending of demo");
      SceneTransitions.LoadSceneAsync(_demo_end_scene);
    }
    else {
      Print.Info("This would be the end of the demo. But this version is not a demo version");
    }
  }
}
