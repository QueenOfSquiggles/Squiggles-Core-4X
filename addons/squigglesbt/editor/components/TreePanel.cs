namespace SquigglesBT;

using System.Collections.Generic;
using Godot;
using SquigglesBT.Nodes;

public partial class TreePanel : PanelContainer {
  [Signal] public delegate void OnParamUpdateEventHandler(string key, Variant value);
  [Export] private Control _childrenPanel;
  [Export] private Label _nodeTypeLabel;
  [Export] private LineEdit _nodeNameLabel;
  [Export] private Control _params;

  private readonly Dictionary<string, Variant> _paramsDict = new();
  private BTNode _node;

  public override void _Ready() => OnParamUpdate += HandleOnParamsUpdated;

  public void LoadSettings(BTNode btnode) {
    if (btnode is null) {
      GD.PrintErr("Panel was attempted to be made with a null BTNode");
      return;
    }
    _node = btnode;
    if (_nodeTypeLabel is not null) {
      _nodeTypeLabel.Text = btnode.GetType().Name;
    }

    if (_nodeNameLabel is not null) {
      _nodeNameLabel.Text = btnode.Label;
      _nodeNameLabel.TextChanged += (text) => btnode.Label = text;
    }
    GD.Print($"Panel loaded with BTNode = {btnode.Label}");
    LoadParameters();
  }

  private void HandleOnParamsUpdated(string key, Variant value) {
    if (_node is null) {
      return;
    }

    _node.Params[key] = value;
  }

  public void LoadParameters() {
    if (_node is null) {
      return;
    }

    var @params = _node.GetKnownParams();
    if (@params.Count <= 0) {
      return; // no params
    }

    foreach (var entry in @params) {
      MakeInspectorFor(entry.Key, entry.Value);
    }
  }

  private void MakeInspectorFor(string key, Variant value) {
    GD.Print($"Handling inspector for {_nodeNameLabel}::{key} == {value}");
    Control control = new HBoxContainer {
      SizeFlagsHorizontal = SizeFlags.ExpandFill
    };
    var lbl = new Label {
      Text = key
    };
    control.AddChild(lbl);

    if (value.VariantType == Variant.Type.Int) {

      var spin = new SpinBox {
        Step = 1.0f,
        Value = value.AsInt32(),
        Suffix = "i",
        SizeFlagsHorizontal = SizeFlags.ExpandFill
      };
      spin.ValueChanged += (v) => EmitSignal(nameof(OnParamUpdate), key, (int)v);
      control.AddChild(spin);
    }
    else if (value.VariantType == Variant.Type.Float) {
      var spin = new SpinBox {
        Step = 0.0001f, // very likely smallest quantity necessary
        Value = value.AsSingle(),
        Suffix = "f",
        SizeFlagsHorizontal = SizeFlags.ExpandFill,

      };
      spin.ValueChanged += (v) => EmitSignal(nameof(OnParamUpdate), key, (float)v);
      control.AddChild(spin);
    }
    else if (value.VariantType == Variant.Type.String) {
      var line = new LineEdit {
        Text = value.AsString(),
        SizeFlagsHorizontal = SizeFlags.ExpandFill,
        ExpandToTextLength = true,
      };
      line.TextChanged += (v) => EmitSignal(nameof(OnParamUpdate), key, v);
      control.AddChild(line);
    }
    // TODO handle new types
    else {
      GD.PrintErr($"Unsupported parameter type: typeof({key}) == {value.VariantType}");
    }
    _params?.AddChild(control);
  }

  public Control GetChildrenPanel() => _childrenPanel;

}
