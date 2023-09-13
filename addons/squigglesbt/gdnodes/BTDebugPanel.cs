using System;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class BTDebugPanel : Node
{

    [Export] private Dictionary DebugDisplay = new();
    public void UpdateDisplay(Blackboard bb)
    {
        var values = bb
            .GetValuesLocal()
            .ToList();
        foreach (var val in values)
        {
            var key = val.Key.Replace("debug", "");
            DebugDisplay[key] = val.Value;
        }
    }
}
