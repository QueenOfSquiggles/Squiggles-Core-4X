namespace Squiggles.Core.Extension;

using System;


/// <summary>
/// SC4X extensions for the Godot.Array class
/// </summary>
public static class ArrayExtensions {
  /// <summary>
  /// Creates a debug string out of the array using the specified delimiter
  /// </summary>
  /// <param name="arr">the array</param>
  /// <param name="delim">the delimiter string to use. defaults to ", "</param>
  /// <returns>a string with the delimiter separated values contained in the array.</returns>
  public static string ToDebugString(this Array arr, string delim = ", ") {
    var str = "[";
    for (var i = 0; i < arr.Length; i++) {
      if (i != 0) {
        str += delim;
      }

      var obj = arr.GetValue(i);
      str += obj is null ? "null" : obj.ToString();
    }
    return str + "]";
  }
}
