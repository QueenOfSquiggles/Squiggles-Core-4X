namespace Squiggles.Core.Scenes.ItemSystem;

using System;
using Godot;
using Squiggles.Core.Extension;

/// <summary>
/// An Area3D based item collector. Used to detect if a item can be picked up and then emits a signal with the data
/// </summary>
[GlobalClass]
public partial class ItemCollector : Area3D {

  /// <summary>
  /// A signal for handling with an item is picked up
  /// </summary>
  /// <param name="nodeRef">a reference to the node that is able to be picked up.</param>
  [Signal] public delegate void OnItemPickupEventHandler(Node3D nodeRef);
  /// <summary>
  /// Emitted when an item/node is rejected from being picked up (for any number of reasons)
  /// </summary>
  /// <param name="nodeRef"></param>
  [Signal] public delegate void OnItemRejectedEventHandler(Node3D nodeRef);

  /// <summary>
  /// The node group that is used to determine whether it is an item
  /// </summary>
  [Export] public string ItemGroupName = "item";

  /// <summary>
  /// Sets a list of groups that the item must be in to be accepted
  /// </summary>
  [Export] public string[] GroupFilters = Array.Empty<string>();

  /// <summary>
  /// A list of item IDs that are accepted.
  /// </summary>
  [Export] public string[] IDFilters = Array.Empty<string>();

  /// <summary>
  /// Whether or not this node is currently processing or not.
  /// </summary>
  [Export] public bool Enabled = true;

  private void OnBodyEnter(Node3D node) {
    if (!Enabled) {
      return;
    }

    if (!node.IsInGroup(ItemGroupName)) {
      return;
    }

    if (node.GetComponent<WorldItemComponent>() is not WorldItemComponent wic) {
      return;
    }

    if (!CheckIDFilter(node, wic) || !CheckGroupFilter(node)) {
      EmitSignal(nameof(OnItemRejected), node);
      return;
    }

    // in item group and passing all filters (or no filters are applied)
    EmitSignal(nameof(OnItemPickup), node);
    return;
  }

  private bool CheckIDFilter(Node node, WorldItemComponent wic) {
    var passingIDFilter = IDFilters.IsEmpty();
    foreach (var id in IDFilters) {
      if (wic.ItemID == id) {
        passingIDFilter = true;
        break;
      }
    }
    return passingIDFilter;
  }

  private bool CheckGroupFilter(Node node) {
    foreach (var f in GroupFilters) {
      if (!node.IsInGroup(f)) {
        EmitSignal(nameof(OnItemRejected), node);
        return false;
      }
    }
    return true;
  }
}
