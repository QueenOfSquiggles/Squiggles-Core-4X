using Godot;
using System;

public class SliderComboInterface
{

	private Node _Node;

	public SliderComboInterface(Node node)
	{
		_Node = node;
	}

	public string Text
	{
		get => _Node.Get("text").AsString();
		set => _Node.Set("text", value);
	}

	public float SliderValue
	{
		get => _Node.Get("slider_value").AsSingle();
		set => _Node.Set("slider_value", value);
	}

	public float MinValue
	{
		get => _Node.Get("min_value").AsSingle();
		set => _Node.Set("min_value", value);
	}

	public float MaxValue
	{
		get => _Node.Get("max_value").AsSingle();
		set => _Node.Set("max_value", value);
	}

	public float StepValue
	{
		get => _Node.Get("step_value").AsSingle();
		set => _Node.Set("step_value", value);
	}

	/*
	@export var min_value : float = 0.0 :
	@export var max_value : float = 1.0 :
	@export var step_value : float = 0.1 :
	*/
}