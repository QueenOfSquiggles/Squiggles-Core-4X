using Godot;
using queen.error;

namespace SquiggleZoneGameBase.Modules.Extensions;

public static class StringExtensions
{

    public static float[] ParseFloatsSquiggles(this string str, string delim = ",")
    {
        string chunk = str;
        for (int i = 0; i < str.Length; i++) if (char.IsDigit(str[i])) { chunk = str[i..]; break; }
        for (int i = chunk.Length - 1; i >= 0; i--) if (char.IsDigit(chunk[i])) { chunk = chunk[..i]; break; }
        var parts = chunk.Split(delim);
        var floats = new float[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            if (float.TryParse(parts[i], out float val)) floats[i] = val;
            else floats[i] = 0.0f;
        }
        return floats;
    }

}
