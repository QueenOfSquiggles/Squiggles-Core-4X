namespace Squiggles.Core.Scenes.ItemSystem;

using Godot;

/// <summary>
/// A component that stores a <see cref="WorldEntity"/> id. Used in conjunction with the <see cref="RegistrationManager"/> to dynamically load item information in a way that supports modded items.
/// </summary>
[GlobalClass]
public partial class WorldItemComponent : Node {
  [Export] public string ItemID = "";

}
