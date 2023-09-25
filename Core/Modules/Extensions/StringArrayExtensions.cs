namespace Squiggles.Core.Extension;

/// <summary>
/// SC4X System.Collection.Generic.Array(string)
/// </summary>
public static class StringArrayExtensions {

  /// <summary>
  /// Produces a debugging string that can be printed like Godot.Array's would normally in GDScript
  /// I would love to adapt this to use generics so any type is valid but IDK how I would do that. I'm so tired.
  /// </summary>
  /// <param name="arr"></param>
  /// <returns></returns>
  public static string ToDebugString(this string[] arr) {
    var composite = "[";
    if (arr.Length > 0) {
      composite += arr[0];
    }

    if (arr.Length > 1) {
      for (var i = 1; i < arr.Length; i++) {
        composite += $", {arr[i]}";
      }
    }
    composite += "]";
    return composite;

  }

}
