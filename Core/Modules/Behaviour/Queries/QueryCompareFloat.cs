namespace Squiggles.Core.BT;
using Godot;
using SquigglesBT;
using SquigglesBT.Nodes;

public class QueryCompareFloat : Leaf {

  protected override void RegisterParams() {
    Params["target"] = 0.0f;
    Params["value"] = 0.0f;
    Params["op"] = "==";
  }

  public override int Tick(Node actor, Blackboard blackboard) {
    var target = GetParam("target", 0.0f, blackboard).AsSingle();
    var value = GetParam("value", 0.0f, blackboard).AsSingle();
    var op = GetParam("op", 0.0f, blackboard).AsString();
    return op switch {
      "==" => value == target ? SUCCESS : FAILURE,
      "<=" => value <= target ? SUCCESS : FAILURE,
      ">=" => value >= target ? SUCCESS : FAILURE,
      "<" => value < target ? SUCCESS : FAILURE,
      ">" => value > target ? SUCCESS : FAILURE,
      "!=" => value != target ? SUCCESS : FAILURE,
      _ => FAILURE
    };
  }

}
