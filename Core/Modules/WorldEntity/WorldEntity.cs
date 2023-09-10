using System;
using Godot;

[GlobalClass, Icon("res://Assets/UsefulEditorIcons/world_entity_resource.svg")]
public partial class WorldEntity : Resource
{

    [Export] public string ID = "generic_id";
    [Export] public bool CanCollect = true;
    [Export] public Texture InventoryIcon;
    [Export] public PackedScene WorldScene;
    [Export] public int MarketValue = 1;
    [Export] public float MarketValueRange = 2.5f;

}
