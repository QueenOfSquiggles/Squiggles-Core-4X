using Godot;
using System;

public partial class VersioningLabel : Label
{
    [Export] private string text_release = "Full Release";
    [Export] private string text_demo = "Demo Version";

    public override void _Ready()
    {
        if (OS.HasFeature("demo"))
            Text = text_demo;
        else 
            Text = text_release;
        #if DEBUG
            Text += " - debug";
        #endif
    }
    

}
