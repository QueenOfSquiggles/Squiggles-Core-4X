namespace SquigglesBT.Nodes;
using Godot;

public class ClockLimiter : Decorator {
  private float _counter = float.MaxValue;


  protected override void RegisterParams() {
    Params["seconds"] = 1.0f;
    Params["startat"] = 0.0f;
  }

  public override int Tick(Node actor, Blackboard blackboard) {
    if (Children.Count <= 0 || !Params.ContainsKey("seconds")) {
      return FAILURE;
    }

    var p_counter = GetParam("seconds", 0.0f, blackboard).AsSingle();
    if (_counter > p_counter) {
      _counter = p_counter;
    }

    _counter -= blackboard.GetLocal("delta").AsSingle();
    if (_counter <= 0f) {
      var result = Children[0].Tick(actor, blackboard);
      if (result != RUNNING) {
        _counter = p_counter;
      }

      return result;
    }
    return SUCCESS;
  }

  public override void LoadDebuggingValues(Blackboard bb)
    => bb.SetLocal($"debug.{Label}:counter", _counter);
}
