namespace SquigglesBT.Nodes;

using System.Linq;
using Godot;
using Godot.Collections;

public partial class BTDebugPanel : Node {

  [Export] private Dictionary _debugDisplay = new();
  public void UpdateDisplay(Blackboard bb) {
    var values = bb
        .GetValuesLocal()
        .ToList();
    foreach (var val in values) {
      var key = val.Key.Replace("debug", "");
      _debugDisplay[key] = val.Value;
    }
  }
}
