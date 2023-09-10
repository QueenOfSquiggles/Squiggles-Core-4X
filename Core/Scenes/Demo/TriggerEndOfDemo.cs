using Godot;
using queen.error;
using System;

public partial class TriggerEndOfDemo : Node
{
    [Export(PropertyHint.File, "*.tscn")] private string demo_end_scene = "";

    public void EndTheDemo()
    {
        if (OS.HasFeature("demo"))
        {
            Print.Info("Demo has completed. Ending of demo");
            Scenes.LoadSceneAsync(demo_end_scene);
        } else {
            Print.Info("This would be the end of the demo. But this version is not a demo version");
        }
    }
}
