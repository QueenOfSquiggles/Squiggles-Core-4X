using Godot;
using queen.data;
using System;

public partial class LoadControlsMappings : Node
{

    public override void _Ready()
    {
        // doesn't really have to be anything. Just forces the instance to be initialized, which involves loading from disk
        Controls.Instance.GetCurrentMappingFor("ui_accept");
    }
}
