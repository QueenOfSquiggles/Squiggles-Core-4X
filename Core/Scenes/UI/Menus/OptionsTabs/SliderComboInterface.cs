namespace Squiggles.Core.Scenes.UI.Menus;

using System;
using Godot;

/// <summary>
/// A C# adapter for the GDScript class SliderCombo ("res://Core/Scenes/UI/Menus/OptionsTabs/SliderCombo.gd"). Since interfacing with GDScript directly can be a bit of a pain, and I use the SliderCombo EVERYWHERE, this adapter takes in a node and provides a series of properties that allow get/set operations on the node, provided it is indeed an instance of the SliderCombo GDScript class.
/// </summary>
public class SliderComboAdapter {

  private const string PATH_SLIDER_COMBO_GD = "res://Core/Scenes/UI/Menus/OptionsTabs/SliderCombo.gd";

  private readonly Node _node;

  /// <summary>
  /// Creates a new SliderComboAdapter
  /// </summary>
  /// <param name="node">the node being passed in. Should be an instance of SliderCombo for this to work.</param>
  /// <exception cref="InvalidCastException">Throws an exception if the passed in node is not valid as a SliderCombo GDScript class</exception>
  public SliderComboAdapter(Node node) {
    if (node.GetScript().AsGodotObject() is not Script script || !script.ResourcePath.Contains("SliderCombo.gd")) {
      throw new InvalidCastException("SliderComboAdapter requires that 'node' parameter be of the gdscript class 'SliderCombo'");
    }
    _node = node;
  }

  /// <summary>
  /// Handles the "text" property of ComboSlider
  /// </summary>
  public string Text {
    get => _node.Get("text").AsString();
    set => _node.Set("text", value);
  }

  /// <summary>
  /// Handles the "slider_value" property of ComboSlider
  /// </summary>
  public float SliderValue {
    get => _node.Get("slider_value").AsSingle();
    set => _node.Set("slider_value", value);
  }

  /// <summary>
  /// Handles the "min_value" property of ComboSlider
  /// </summary>
  public float MinValue {
    get => _node.Get("min_value").AsSingle();
    set => _node.Set("min_value", value);
  }

  /// <summary>
  /// Handles the "max_value" property of ComboSlider
  /// </summary>
  public float MaxValue {
    get => _node.Get("max_value").AsSingle();
    set => _node.Set("max_value", value);
  }

  /// <summary>
  /// Handles the "step_value" property of ComboSlider
  /// </summary>
  public float StepValue {
    get => _node.Get("step_value").AsSingle();
    set => _node.Set("step_value", value);
  }
}
