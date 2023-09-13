using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace queen.extension;

public static class DictionaryExtensions
{
    public static void AddSafe<T, Y>(this Dictionary<T, Y> dictionary, T key, Y value)
        where T : notnull
    {

        if (dictionary.ContainsKey(key)) dictionary[key] = value;
        else dictionary.Add(key, value);
    }

    public static bool ContainsKeys<T, Y>(this Dictionary<T, Y> dictionary, params T[] keys)
        where T : notnull
    {
        foreach (var k in keys)
        {
            if (!dictionary.ContainsKey(k)) return false;
        }
        return true;
    }

}