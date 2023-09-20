namespace Squiggles.Core.BT;

using Godot;
using Squiggles.Core.Error;
using SquigglesBT;
using SquigglesBT.Nodes;

public class ActorPlayAnim : Leaf {

  protected override void RegisterParams() {
    Params["track"] = "track name";
    Params["reverse"] = false;
  }

  public override int Tick(Node actor, Blackboard blackboard) {
    var path = blackboard.GetLocal("anim").AsNodePath();
    if (actor.GetNode(path) is not AnimationPlayer anim) {
      Print.Error($"Failed to find node from path {path} for behaviour tree");
      return FAILURE;
    }
    var track = GetParam("track", "track_name", blackboard).AsString();

    if (GetParam("reverse", false, blackboard).AsBool()) {
      anim.PlayBackwards(track);
    }
    else {
      anim.Play(track);
    }

    return SUCCESS;
  }

}
