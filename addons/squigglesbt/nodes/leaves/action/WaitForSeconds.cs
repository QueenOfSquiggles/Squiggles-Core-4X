namespace SquigglesBT.Nodes;
using Godot;

public class WaitForSeconds : Leaf {
  private float _counter = -1f;

  protected override void RegisterParams() => Params["seconds"] = 1.0f;

  public override int Tick(Node actor, Blackboard blackboard) {
    if (!Params.ContainsKey("seconds")) {
      return FAILURE;
    }

    _counter -= blackboard.GetLocal("delta").AsSingle();

    if (_counter <= 0f) {
      _counter = GetParam("seconds", 1.0f, blackboard).AsSingle();
      return SUCCESS;
    }
    return RUNNING;
  }

  public override void LoadDebuggingValues(Blackboard bb) {
    bb.SetLocal($"debug.{Label}:param.seconds", Params["seconds"]);
    bb.SetLocal($"debug.{Label}:counter", _counter);
  }
}
