namespace Squiggles.Core.Extension;

using System;


public static class ArrayExtensions {
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
