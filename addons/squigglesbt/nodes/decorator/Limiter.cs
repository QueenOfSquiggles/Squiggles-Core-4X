namespace SquigglesBT.Nodes;
using Godot;

public class Limiter : Decorator {
  private int _counter = int.MaxValue;

  public override int Tick(Node actor, Blackboard blackboard) {
    if (Children.Count <= 0) {
      return FAILURE;
    }

    var p_counter = GetParam("count", 5, blackboard).AsInt32();
    if (_counter > p_counter) {
      _counter = p_counter;
    }

    if (_counter <= 0) {
      return FAILURE;
    }

    var result = Children[0].Tick(actor, blackboard);
    if (result != RUNNING) {
      _counter--;
    }

    return result;
  }

  public override void LoadDebuggingValues(Blackboard bb)
    => bb.SetLocal($"debug.{Label}:counter", _counter);

  protected override void RegisterParams() => Params["count"] = 5;

}
