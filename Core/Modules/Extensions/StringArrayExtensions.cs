namespace Squiggles.Core.Extension;

public static class StringArrayExtensions {

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
