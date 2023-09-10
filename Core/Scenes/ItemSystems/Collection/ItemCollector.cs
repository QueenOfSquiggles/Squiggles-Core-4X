using System;
using Godot;
using queen.error;
using queen.extension;

public partial class ItemCollector : Area3D
{

    [Signal] public delegate void OnItemPickupEventHandler(Node3D nodeRef);
    [Signal] public delegate void OnItemRejectedEventHandler(Node3D nodeRef);

    [Export] public string ItemGroupName = "item";
    [Export] public string[] GroupFilters = Array.Empty<string>();
    [Export] public string[] IDFilters = Array.Empty<string>();
    [Export] public bool Enabled = true;

    private void OnBodyEnter(Node3D node)
    {
        if (!Enabled) return;
        if (!node.IsInGroup(ItemGroupName)) return;
        if (node.GetComponent<WorldItemComponent>() is not WorldItemComponent wic) return;

        if (!CheckIDFilter(node, wic) || !CheckGroupFilter(node))
        {
            EmitSignal(nameof(OnItemRejected), node);
            return;
        }

        // in item group and passing all filters (or no filters are applied)
        EmitSignal(nameof(OnItemPickup), node);
        return;
    }

    private bool CheckIDFilter(Node node, WorldItemComponent wic)
    {
        bool passingIDFilter = IDFilters.IsEmpty();
        foreach (string id in IDFilters)
        {
            if (wic.ItemID == id)
            {
                passingIDFilter = true;
                break;
            }
        }
        return passingIDFilter;
    }

    private bool CheckGroupFilter(Node node)
    {
        foreach (var f in GroupFilters)
        {
            if (!node.IsInGroup(f))
            {
                EmitSignal(nameof(OnItemRejected), node);
                return false;
            }
        }
        return true;
    }
}
