namespace SquigglesBT.Nodes;
using Godot;

public class SequenceStar : Compositor {
  private int _current = -1;

  public override int Tick(Node actor, Blackboard blackboard) {
    if (_current >= 0) {
      var result = Children[0].Tick(actor, blackboard);
      if (result != RUNNING) {
        _current = -1;
      }

      return result;
    }
    foreach (var c in Children) {
      var result = c.Tick(actor, blackboard);
      if (result == RUNNING) {
        _current = Children.IndexOf(c);
      }

      if (result != FAILURE) {
        return result;
      }
    }
    return FAILURE;
  }

  protected override void RegisterParams() {
  }

}
