namespace Squiggles.Core.Extension;

using System.Collections.Generic;


public static class DictionaryExtensions {
  public static void AddSafe<T, TY>(this Dictionary<T, TY> dictionary, T key, TY value)
      where T : notnull {

    if (dictionary.ContainsKey(key)) {
      dictionary[key] = value;
    }
    else {
      dictionary.Add(key, value);
    }
  }

  public static bool ContainsKeys<T, TY>(this Dictionary<T, TY> dictionary, params T[] keys)
      where T : notnull {
    foreach (var k in keys) {
      if (!dictionary.ContainsKey(k)) {
        return false;
      }
    }
    return true;
  }

}
