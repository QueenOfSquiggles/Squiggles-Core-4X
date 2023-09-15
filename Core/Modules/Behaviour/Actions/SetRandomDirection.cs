namespace Squiggles.Core.BT;
using System;
using Godot;
using SquigglesBT;
using SquigglesBT.Nodes;

public class SetRandomDirection : Leaf {
  private readonly Random _random = new();
  private Vector3 _lastDir = Vector3.Zero;

  protected override void RegisterParams() => Params["target"] = "key";

  public override int Tick(Node actor, Blackboard blackboard) {
    var target = GetParam("target", "key", blackboard).AsString();
    var dir = new Vector3 {
      X = (_random.NextSingle() - 0.5f) * 2.0f,
      Z = (_random.NextSingle() - 0.5f) * 2.0f
    }.Normalized();
    blackboard.SetLocal(target, dir);
    _lastDir = dir;
    return SUCCESS;
  }

  public override void LoadDebuggingValues(Blackboard bb) => bb.SetLocal($"debug.{Label}:last_dir", _lastDir);
}
