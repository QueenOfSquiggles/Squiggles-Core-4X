namespace Squiggles.Core.Extension;
using System;


/// <summary>
/// SC4X System.Random extensions
/// </summary>
public static class RandomExtensions {
  /// <summary>
  /// Calculates a boolean based on a percent chance that it is true. Useful for simulating "dice rolls"
  /// </summary>
  /// <param name="random"></param>
  /// <param name="percent">the percentage (0.0-1.0) chance of being true</param>
  /// <returns></returns>
  public static bool PercentChance(this Random random, float percent) => random.NextSingle() < percent;

  /// <summary>
  /// Unfortunately not a true guassian distribution. It produces a float value from -1.0 to 1.0
  /// For the life of me I cannot figure out what the name would be for this value otherwise, but I have experienced guassians as the same range so it's a naming approximation. If some super smart math person wants some free headpats I'd love to know what this is supposed to be called!!!
  /// </summary>
  /// <param name="random"></param>
  /// <returns></returns>
  public static float NextGuass(this Random random) => (random.NextSingle() - 0.5f) * 2f;

  /// <summary>
  /// Why in the fuck does System.Random not have a NextBool method!!??? It's literally not hard and it's super useful.
  /// </summary>
  /// <param name="random"></param>
  /// <returns></returns>
  public static bool NextBool(this Random random) => random.Next() % 2 == 0;

  public static int NextRange(this Random random, int max, int min = 0) => (random.Next() % (max - min + 1)) + min;
}
