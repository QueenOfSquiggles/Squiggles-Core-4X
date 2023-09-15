namespace Squiggles.Core.Scenes.UI.Menus;
using Godot;

public class SliderComboInterface {

  private readonly Node _node;

  public SliderComboInterface(Node node) {
    _node = node;
  }

  public string Text {
    get => _node.Get("text").AsString();
    set => _node.Set("text", value);
  }

  public float SliderValue {
    get => _node.Get("slider_value").AsSingle();
    set => _node.Set("slider_value", value);
  }

  public float MinValue {
    get => _node.Get("min_value").AsSingle();
    set => _node.Set("min_value", value);
  }

  public float MaxValue {
    get => _node.Get("max_value").AsSingle();
    set => _node.Set("max_value", value);
  }

  public float StepValue {
    get => _node.Get("step_value").AsSingle();
    set => _node.Set("step_value", value);
  }

  /*
	@export var min_value : float = 0.0 :
	@export var max_value : float = 1.0 :
	@export var step_value : float = 0.1 :
	*/
}
