using System;
using Godot;

public partial class CharStatFloat : Node
{
    public enum Modifier
    {
        REPLACE, ADD, MULTIPLY
    };

    [Export] public Modifier StatMod = Modifier.REPLACE;
    [Export] public float StoredValue = 0.0f;

    public float GetNetValue()
    {
        var val = StoredValue;
        foreach (var n in GetChildren())
        {
            if (n is not CharStatFloat csf) continue;
            val = csf.ApplyValue(val);
        }
        return val;
    }

    public float ApplyValue(float inVal)
    {
        switch (StatMod)
        {
            case Modifier.REPLACE:
                return StoredValue;
            case Modifier.ADD:
                return inVal + StoredValue;
            case Modifier.MULTIPLY:
                return inVal * StoredValue;
            default:
                break;
        }
        return inVal;
    }

}
