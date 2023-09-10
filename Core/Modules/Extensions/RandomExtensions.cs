using System;

namespace queen.extension;

public static class RandomExtensions
{
    public static bool PercentChance(this Random random, float percent)
    {
        return random.NextSingle() < percent;
    }

    public static float NextGuass(this Random random)
    {
        return (random.NextSingle() - 0.5f) * 2f;
    }

    public static bool NextBool(this Random random)
    {
        return random.Next() % 2 == 0;
    }
}
