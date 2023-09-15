namespace Squiggles.Core.Extension;
using System;


public static class RandomExtensions {
  public static bool PercentChance(this Random random, float percent) => random.NextSingle() < percent;

  public static float NextGuass(this Random random) => (random.NextSingle() - 0.5f) * 2f;

  public static bool NextBool(this Random random) => random.Next() % 2 == 0;
}
