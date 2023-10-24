namespace Squiggles.Core.Extension;

using System.Collections.Generic;


/// <summary>
/// SC4X System.Collections.Generic.Dictionary extensions
/// </summary>
public static class DictionaryExtensions {
  /// <summary>
  /// Safely adds a value to a dictionary.
  /// </summary>
  /// <typeparam name="T">type of the dictionary's keys</typeparam>
  /// <typeparam name="TY">type of the dictionary's values</typeparam>
  /// <param name="dictionary">the dictionary in question</param>
  /// <param name="key">the key to insert at</param>
  /// <param name="value">the value to insert</param>
  public static void AddSafe<T, TY>(this Dictionary<T, TY> dictionary, T key, TY value) where T : notnull {

    if (dictionary.ContainsKey(key)) {
      dictionary[key] = value;
    }
    else {
      dictionary.Add(key, value);
    }
  }

  /// <summary>
  /// Determines whether a large number of keys are present in a given dictionary.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <typeparam name="TY"></typeparam>
  /// <param name="dictionary"></param>
  /// <param name="keys">an array of keys of the type from the specified dictionary.</param>
  /// <returns></returns>
  public static bool ContainsKeys<T, TY>(this Dictionary<T, TY> dictionary, params T[] keys) where T : notnull {
    foreach (var k in keys) {
      if (!dictionary.ContainsKey(k)) {
        return false;
      }
    }
    return true;
  }

}
