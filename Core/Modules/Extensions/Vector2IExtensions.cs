namespace Squiggles.Core.Extension;
using Godot;
using Squiggles.Core.Math;

public static class Vector2IExtensions {

  public static InventoryPosition ToInventoryPosition(this Vector2I vec) => InventoryPosition.FromVector2I(vec);

}
