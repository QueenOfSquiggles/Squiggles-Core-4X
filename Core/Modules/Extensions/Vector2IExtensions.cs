namespace Squiggles.Core.Extension;
using Godot;
using Squiggles.Core.Math;

/// <summary>
/// SC4X Godot.Vector2I extensions
/// </summary>
public static class Vector2IExtensions {

  /// <summary>
  /// Converts the Vector2I into a <see cref="InventoryPosition"/>. Whether or not this is actually useful is yet to be seen.
  /// Was originally part of a workflow where I was building a Resident Evil 7 clone. That project got scrapped because mental instability go brrrrrrr ;(
  /// </summary>
  /// <param name="vec"></param>
  /// <returns></returns>
  public static InventoryPosition ToInventoryPosition(this Vector2I vec) => InventoryPosition.FromVector2I(vec);

}
