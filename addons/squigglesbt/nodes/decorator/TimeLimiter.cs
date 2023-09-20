namespace SquigglesBT.Nodes;

using Godot;

public class TimeLimiter : Decorator {


  private float _counter = int.MaxValue;


  protected override void RegisterParams() => Params["seconds"] = 1.0f;

  public override int Tick(Node actor, Blackboard blackboard) {
    if (Children.Count <= 0) {
      return FAILURE;
    }

    var p_counter = GetParam("seconds", 0.0f, blackboard).AsSingle();
    if (_counter > p_counter) {
      _counter = p_counter;
    }

    if (_counter <= 0) {
      return FAILURE;
    }

    _counter -= blackboard.GetLocal("delta").AsSingle();
    var result = Children[0].Tick(actor, blackboard);
    if (result != RUNNING) {
      _counter = -1;
    }

    return result;
  }

  public override void LoadDebuggingValues(Blackboard bb) => bb.SetLocal($"debug.{Label}:counter", _counter);
}
