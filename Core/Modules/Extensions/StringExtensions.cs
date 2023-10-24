namespace Squiggles.Core.Extension;

/// <summary>
/// SC4X string extensions
/// </summary>
public static class StringExtensions {

  /// <summary>
  /// A custom implementation used interally to parse an array of floats from a string using a delimiter.
  /// </summary>
  /// <param name="str">the string in question</param>
  /// <param name="delim">the delimiter to expect</param>
  /// <returns>an array of floats parsed from the string</returns>.
  public static float[] ParseFloatsSquiggles(this string str, string delim = ",") {
    var chunk = str;
    for (var i = 0; i < str.Length; i++) {
      if (char.IsDigit(str[i])) { chunk = str[i..]; break; }
    }

    for (var i = chunk.Length - 1; i >= 0; i--) {
      if (char.IsDigit(chunk[i])) { chunk = chunk[..i]; break; }
    }

    var parts = chunk.Split(delim);
    var floats = new float[parts.Length];
    for (var i = 0; i < parts.Length; i++) {
      floats[i] = float.TryParse(parts[i], out var val) ? val : 0.0f;
    }
    return floats;
  }

}
