namespace Squiggles.Core.Scenes.Demo;
using Godot;
using Squiggles.Core.Error;
using Squiggles.Core.Scenes.Utility;

public partial class TriggerEndOfDemo : Node {
  [Export(PropertyHint.File, "*.tscn")] private string _demo_end_scene = "";

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
