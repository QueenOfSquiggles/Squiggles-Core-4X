namespace SquigglesBT;

using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using SquigglesBT.Nodes;

public partial class BehaviourTreeWindow : Window {
  [Export] private PanelContainer _treeContainerRoot;
  [Export] private Label _currentTreeLabel;
  [Export] private BehaviourTree _loadedTree;
  [Export] private PackedScene _sceneNoChildren;
  [Export] private PackedScene _sceneOneChild;
  [Export] private PackedScene _sceneManyChildren;



  public override void _Ready() {
    if (_loadedTree is not null) {
      LoadTree(_loadedTree);
    }
  }

  public void LoadTree(BehaviourTree tree) {
    _loadedTree = tree;
    if (_loadedTree is null) {
      return;
    }

    _loadedTree.RebuildTree();
    Title = _loadedTree.Name;
    if (_currentTreeLabel is not null) {
      _currentTreeLabel.Text = Title;
    }

    GD.Print("Loading tree");

    foreach (var c in _treeContainerRoot?.GetChildren() ?? new Array<Node>()) {
      c.QueueFree();
      _treeContainerRoot?.RemoveChild(c);
    }

    RebuildTreeVisualsRecursive(PushNode(_treeContainerRoot, _loadedTree.TreeRoot), _loadedTree.TreeRoot);
  }

  private void RebuildTreeVisualsRecursive(Control parent_panel, BTNode parent_node) {
    if (parent_panel is null || parent_node is null) {
      return;
    }

    GD.Print($"Handling panel for node: {parent_node.Label}");
    var target = (parent_panel as TreePanel)?.GetChildrenPanel() ?? parent_panel;
    if (target == null) {
      return;
    }

    foreach (var c in parent_node.Children) {
      var panel = PushNode(target, c);
      RebuildTreeVisualsRecursive(panel, c);
    }
  }

  private TreePanel PushNode(Control parent, BTNode node) {
    if (parent is null || node is null) {
      return null;
    }

    var panel = GetPanelFor(node);
    if (panel is null) {
      return null;
    }

    parent.AddChild(panel);
    panel.LoadSettings(node);
    panel.OnParamUpdate += (key, value) => {
      // TODO refresh display
      // LoadTree(_LoadedTree);
    };
    return panel;
  }

  private TreePanel GetPanelFor(BTNode node) => node.MaxChildren switch {
    0 => _sceneNoChildren?.Instantiate() as TreePanel,
    1 => _sceneOneChild?.Instantiate() as TreePanel,
    _ => _sceneManyChildren?.Instantiate() as TreePanel
  };

  private void SaveResource() {
    if (_loadedTree is null) {
      return;
    }

    var nodes = new List<BTNode>
    {
            _loadedTree.TreeRoot
    };
    while (nodes.Count > 0) {
      var n = nodes.First();
      nodes.Remove(n);
      var p = string.Concat(n.Params
          .ToList()
          .ConvertAll((entry) => $"{entry.Key}={entry.Value}, ")
          .ToArray());
      GD.Print($"{n.Label} {n.GetType()} : {p}");
      nodes.AddRange(n.Children);
    }

    var json_data = Json.Stringify(GenerateData(_loadedTree.TreeRoot), "\t");
    _loadedTree.UpdateResource(json_data);
  }

  private Dictionary GenerateData(BTNode node) {
    Dictionary dict = new() {
      ["type"] = node.GetType().ToString(),
      ["label"] = node.Label
    };
    foreach (var e in node.Params) {
      dict[e.Key] = e.Value;
    }

    var children = new Array();
    foreach (var c in node.Children) {
      children.Add(GenerateData(c));
    }

    dict["children"] = children;
    return dict;
  }


}
