namespace Squiggles.Core.WorldEntity;

using Godot;
using Squiggles.Core.Attributes;

/// <summary>
/// A resource for storing item/entity data. Currently used extensively in my "Cute Farm Sim" game project. May be refactored out to enable SC4X to be more general purpose and less bloaty?
/// </summary>
[MarkForRefactor("Strip 'CuteFarmSim' Vestiges", "This class is used in my cute farm sim project, but currently serves little purpose outside of it")]
[GlobalClass]
public partial class WorldEntity : Resource {

  /// <summary>
  /// The string ID of the world entity
  /// </summary>
  [Export] public string ID = "generic_id";
  /// <summary>
  /// Whether or not the entity can be collected by the player/others
  /// </summary>
  [Export] public bool CanCollect = true;
  /// <summary>
  /// A texture to show in the inventory
  /// </summary>
  [Export] public Texture InventoryIcon;
  /// <summary>
  /// A scene that represents the world-space existence of the entity
  /// </summary>
  [Export] public PackedScene WorldScene;
  /// <summary>
  /// The monetary value of the entity.
  /// </summary>
  [Export] public int MarketValue = 1;
  /// <summary>
  /// How much the <see cref="MarketValue"/> is allowed to fluctuate up/down
  /// </summary>
  [Export] public float MarketValueRange = 2.5f;

}
