using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class TreePanel : PanelContainer
{
    [Signal] public delegate void OnParamUpdateEventHandler(string key, Variant value);
    [Export] private Control _ChildrenPanel;
    [Export] private Label _NodeTypeLabel;
    [Export] private LineEdit _NodeNameLabel;
    [Export] private Control _Params;

    private Dictionary<string, Variant> _ParamsDict = new();
    private BTNode _Node;

    public override void _Ready()
    {
        OnParamUpdate += HandleOnParamsUpdated;
    }

    public void LoadSettings(BTNode btnode)
    {
        if (btnode is null)
        {
            GD.PrintErr("Panel was attempted to be made with a null BTNode");
            return;
        }
        _Node = btnode;
        if (_NodeTypeLabel is not null) _NodeTypeLabel.Text = btnode.GetType().Name;
        if (_NodeNameLabel is not null)
        {
            _NodeNameLabel.Text = btnode.Label;
            _NodeNameLabel.TextChanged += (text) => btnode.Label = text;
        }
        GD.Print($"Panel loaded with BTNode = {btnode.Label}");
        LoadParameters();
    }

    private void HandleOnParamsUpdated(string key, Variant value)
    {
        if (_Node is null) return;
        _Node.Params[key] = value;
    }

    public void LoadParameters()
    {
        if (_Node is null) return;
        var _params = _Node.GetKnownParams();
        if (_params.Count <= 0) return; // no params
        foreach (var entry in _params)
            MakeInspectorFor(entry.Key, entry.Value);
    }

    private void MakeInspectorFor(string key, Variant value)
    {
        GD.Print($"Handling inspector for {_NodeNameLabel}::{key} == {value}");
        Control control = new HBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        var lbl = new Label
        {
            Text = key
        };
        control.AddChild(lbl);

        if (value.VariantType == Variant.Type.Int)
        {

            var spin = new SpinBox
            {
                Step = 1.0f,
                Value = value.AsInt32(),
                Suffix = "i",
                SizeFlagsHorizontal = SizeFlags.ExpandFill
            };
            spin.ValueChanged += (v) => EmitSignal(nameof(OnParamUpdate), key, (int)v);
            control.AddChild(spin);
        }
        else if (value.VariantType == Variant.Type.Float)
        {
            var spin = new SpinBox
            {
                Step = 0.0001f, // very likely smallest quantity necessary
                Value = value.AsSingle(),
                Suffix = "f",
                SizeFlagsHorizontal = SizeFlags.ExpandFill,

            };
            spin.ValueChanged += (v) => EmitSignal(nameof(OnParamUpdate), key, (float)v);
            control.AddChild(spin);
        }
        else if (value.VariantType == Variant.Type.String)
        {
            var line = new LineEdit
            {
                Text = value.AsString(),
                SizeFlagsHorizontal = SizeFlags.ExpandFill,
                ExpandToTextLength = true,
            };
            line.TextChanged += (v) => EmitSignal(nameof(OnParamUpdate), key, v);
            control.AddChild(line);
        }
        // TODO handle new types
        else
        {
            GD.PrintErr($"Unsupported parameter type: typeof({key}) == {value.VariantType}");
        }
        _Params?.AddChild(control);
    }

    public Control GetChildrenPanel() => _ChildrenPanel;

}
