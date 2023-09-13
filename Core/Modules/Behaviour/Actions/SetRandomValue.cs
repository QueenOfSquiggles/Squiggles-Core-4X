using System;
using System.Collections.Generic;
using Godot;
using queen.extension;

public class SetRandomValue : Leaf
{
	private Random _Random = new();
	protected override void RegisterParams()
	{
		Params["val_type"] = "float";
		Params["target"] = "key";
		Params["min"] = 0.0f;
		Params["max"] = 1.0f;
	}

	public override int Tick(Node actor, Blackboard bb)
	{
		var target = GetParam("target", "key", bb).AsString();
		var type = GetParam("val_type", "float", bb).AsString();
		var min = GetParam("min", 0.0f, bb).AsSingle();
		var max = GetParam("min", 1.0f, bb).AsSingle();
		var size = max - min;

		float r() => _Random.NextSingle() * size + min;
		switch (type)
		{
			case "float":
				bb.SetLocal(target, r());
				break;
			case "int":
				bb.SetLocal(target, (int)r());
				break;
			case "bool":
				bb.SetLocal(target, _Random.NextBool());
				break;
			case "Vector2":
				bb.SetLocal(target, new Vector2(r(), r()));
				break;
			case "Vector3":
				bb.SetLocal(target, new Vector3(r(), r(), r()));
				break;
		}
		return SUCCESS;
	}

	public override void LoadDebuggingValues(Blackboard bb)
	{
		bb.SetLocal($"debug.{Label}:last_dir", bb.GetLocal(GetParam("target", "key", bb).AsString()));
	}
}