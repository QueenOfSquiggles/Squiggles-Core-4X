using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public partial class BehaviourTreeWindow : Window
{
    [Export] private PanelContainer _TreeContainerRoot;
    [Export] private Label _CurrentTreeLabel;
    [Export] private BehaviourTree _LoadedTree;
    [Export] private PackedScene _SceneNoChildren;
    [Export] private PackedScene _SceneOneChild;
    [Export] private PackedScene _SceneManyChildren;



    public override void _Ready()
    {
        if (_LoadedTree is not null) LoadTree(_LoadedTree);
    }

    public void LoadTree(BehaviourTree tree)
    {
        _LoadedTree = tree;
        if (_LoadedTree is null) return;
        _LoadedTree.RebuildTree();
        Title = _LoadedTree.Name;
        if (_CurrentTreeLabel is not null) _CurrentTreeLabel.Text = Title;

        GD.Print("Loading tree");

        foreach (var c in _TreeContainerRoot?.GetChildren() ?? new Array<Node>())
        {
            c.QueueFree();
            _TreeContainerRoot?.RemoveChild(c);
        }

        RebuildTreeVisualsRecursive(PushNode(_TreeContainerRoot, _LoadedTree.TreeRoot), _LoadedTree.TreeRoot);
    }

    private void RebuildTreeVisualsRecursive(Control parent_panel, BTNode parent_node)
    {
        if (parent_panel is null || parent_node is null) return;
        GD.Print($"Handling panel for node: {parent_node.Label}");
        var target = (parent_panel as TreePanel)?.GetChildrenPanel() ?? parent_panel;
        if (target == null) return;
        foreach (var c in parent_node.Children)
        {
            var panel = PushNode(target, c);
            RebuildTreeVisualsRecursive(panel, c);
        }
    }

    private TreePanel PushNode(Control parent, BTNode node)
    {
        if (parent is null) return null;
        if (node is null) return null;
        var panel = GetPanelFor(node);
        if (panel is null) return null;
        parent.AddChild(panel);
        panel.LoadSettings(node);
        panel.OnParamUpdate += (key, value) =>
        {
            // TODO refresh display
            // LoadTree(_LoadedTree);
        };
        return panel;
    }

    private TreePanel GetPanelFor(BTNode node) => node.MaxChildren switch
    {
        0 => _SceneNoChildren?.Instantiate() as TreePanel,
        1 => _SceneOneChild?.Instantiate() as TreePanel,
        _ => _SceneManyChildren?.Instantiate() as TreePanel
    };

    private void SaveResource()
    {
        if (_LoadedTree is null) return;
        var nodes = new List<BTNode>
        {
            _LoadedTree.TreeRoot
        };
        while (nodes.Count > 0)
        {
            var n = nodes.First();
            nodes.Remove(n);
            var p = string.Concat(n.Params
                .ToList()
                .ConvertAll((entry) => $"{entry.Key}={entry.Value}, ")
                .ToArray());
            GD.Print($"{n.Label} {n.GetType()} : {p}");
            nodes.AddRange(n.Children);
        }

        var json_data = Json.Stringify(GenerateData(_LoadedTree.TreeRoot), "\t");
        _LoadedTree.UpdateResource(json_data);
    }

    private Dictionary GenerateData(BTNode node)
    {
        var dict = new Dictionary();
        dict["type"] = node.GetType().ToString();
        dict["label"] = node.Label;
        foreach (var e in node.Params) dict[e.Key] = e.Value;
        var children = new Godot.Collections.Array();
        foreach (var c in node.Children) children.Add(GenerateData(c));
        dict["children"] = children;
        return dict;
    }


}
